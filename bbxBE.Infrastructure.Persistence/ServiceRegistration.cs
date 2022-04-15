using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Caches;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Migrations;
using bbxBE.Infrastructure.Persistence.Repositories;
using bbxBE.Infrastructure.Persistence.Repository;
using FluentMigrator.Runner;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace bbxBE.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // EF connection
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("bbxdbconnection"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }

            // Connection Dappernak
            services.AddSingleton<DapperContext>();

            //Connection DB létrehozásnak
            services.AddSingleton<Database>();

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IUSR_USERRepositoryAsync, USR_USERRepositoryAsync>();
            services.AddTransient<ICustomerRepositoryAsync, CustomerRepositoryAsync>();
            services.AddTransient<IProductGroupRepositoryAsync, ProductGroupRepositoryAsync>();
            services.AddTransient<IOriginRepositoryAsync, OriginRepositoryAsync>();
            services.AddTransient<IProductRepositoryAsync, ProductRepositoryAsync>();
            services.AddTransient<IWarehouseRepositoryAsync, WarehouseRepositoryAsync>();
            services.AddTransient<ICounterRepositoryAsync, CounterRepositoryAsync>();
            services.AddTransient<IInvoiceRepositoryAsync, InvoiceRepositoryAsync>();
            services.AddTransient<IVatRateRepositoryAsync, VatRateRepositoryAsync>();
            services.AddSingleton<ICacheService<Product>, ProductCacheService>();

            /*

                                    Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.Contains("Repository")).ToList().ForEach((t) =>
                                    {
                                        services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
                                    });
                        */
            services.AddLogging(c => c.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddSqlServer2012()
                .WithGlobalConnectionString(configuration.GetConnectionString("bbxdbconnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
           
        }
    }
}