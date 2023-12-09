using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace bbxBE.WebApi
{
    internal static class SerilogExt
    {
        public static IHostBuilder UseSerilogLogger(this IHostBuilder hostBuilder, Serilog.ILogger logger = null, bool dispose = false)
        {
            hostBuilder.ConfigureServices((context, collection) =>
            {
                collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, dispose));
            });
            return hostBuilder;
        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {
            //Read Configuration from appSettings
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            //Initialize Logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .WriteTo.Console()
                .CreateLogger();

            var host = CreateHostBuilder(args)
                .UseSerilog() // using Microsoft...ILogger<type> interface in DI, but with Serilog
                .Build();

            host.MigrateDatabase();


            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                try
                {
                    Log.Information("Application Starting");
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "An error occurred starting the application");
                }

                refreshCaches(services);

            }

            host.Run();


        }

        private static void refreshCaches(IServiceProvider services)
        {
            //***********
            //* Product *
            //***********
            var prodCache = services.GetService<ICacheService<Product>>();
            Task.Run(() => prodCache.RefreshCache()).Wait();

            //***********
            //* VatRate *
            //***********
            var vatCache = services.GetService<ICacheService<VatRate>>();
            Task.Run(() => vatCache.RefreshCache()).Wait();

            //************
            //* Customer *
            //************
            var custCache = services.GetService<ICacheService<Customer>>();
            Task.Run(() => custCache.RefreshCache()).Wait();


            //**********
            //* Origin *
            //**********
            var originCache = services.GetService<ICacheService<Origin>>();
            Task.Run(() => originCache.RefreshCache()).Wait();

            //****************
            //* ProductGroup *
            //****************
            var productGroupCache = services.GetService<ICacheService<ProductGroup>>();
            Task.Run(() => productGroupCache.RefreshCache()).Wait();

        }


        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            config.Sources.Clear();

                            var env = hostingContext.HostingEnvironment;

                            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                                 optional: true, reloadOnChange: true);

                            config.AddEnvironmentVariables();

                            if (args != null)
                            {
                                config.AddCommandLine(args);
                            }
                        })
            .UseSerilog() //Uses Serilog instead of default .NET Logger
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();

            });
    }
}