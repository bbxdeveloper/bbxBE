using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using LinqKit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace bbxBE.WebApi
{
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

            var host = CreateHostBuilder(args).Build();

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
                finally
                {
                    Log.CloseAndFlush();
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
            var dbc = services.GetService<ApplicationDbContext>();
#if !DEBUG
                    var qProdc = dbc.Product.AsNoTracking()
                         .Include(p => p.ProductCodes).AsNoTracking()
                         .Include(pg => pg.ProductGroup).AsNoTracking()
                         .Include(o => o.Origin).AsNoTracking()
                         .Include(v => v.VatRate).AsNoTracking();
#else
            var qProdc = dbc.Product.AsNoTracking()
                 .Include(p => p.ProductCodes).AsNoTracking()
                 .Include(pg => pg.ProductGroup).AsNoTracking()
                 .Include(o => o.Origin).AsNoTracking()
                 .Include(v => v.VatRate).AsNoTracking(); //`.Take(1000);

#endif
            
            var prodCache = services.GetService<ICacheService<Product>>();
            Task.Run(() => prodCache.RefreshCache(qProdc)).Wait();


            //***********
            //* VatRate *
            //***********
            var qvatc = dbc.VatRate
                        .AsNoTracking()
                        .AsExpandable();
            var vatCache = services.GetService<ICacheService<VatRate>>();
            Task.Run(() => vatCache.RefreshCache(qvatc)).Wait();

            //************
            //* Customer *
            //************
            var qCust = dbc.Customer
                        .AsNoTracking()
                        .AsExpandable();
            var custCache = services.GetService<ICacheService<Customer>>();
            Task.Run(() => custCache.RefreshCache(qCust)).Wait();


            //**********
            //* Origin *
            //**********
            var qOrigin = dbc.Origin
                    .AsNoTracking()
                    .AsExpandable();

            var originCache = services.GetService<ICacheService<Origin>>();
            Task.Run(() => originCache.RefreshCache(qOrigin)).Wait();

            //****************
            //* ProductGroup *
            //****************
            var qProductGroup = dbc.ProductGroup
                            .AsNoTracking()
                            .AsExpandable();
            var productGroupCache = services.GetService<ICacheService<ProductGroup>>();
            Task.Run(() => productGroupCache.RefreshCache(qProductGroup)).Wait();

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