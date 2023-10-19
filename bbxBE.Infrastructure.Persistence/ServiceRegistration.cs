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
using System;
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

                services.AddDbContext<ApplicationQueryDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {

                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(
                       configuration.GetConnectionString("bbxdbconnection"),
                       b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                       ),
                       contextLifetime: ServiceLifetime.Transient,
                        optionsLifetime: ServiceLifetime.Singleton
               );

                services.AddDbContext<ApplicationQueryDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString("bbxdbconnection"),
                        b => b.MigrationsAssembly(typeof(ApplicationQueryDbContext).Assembly.FullName)
                        ),
                        contextLifetime: ServiceLifetime.Singleton,
                         optionsLifetime: ServiceLifetime.Singleton
                );

            }

            // Connection Dappernak
            services.AddSingleton<DapperContext>();

            //Connection DB létrehozásnak
            services.AddSingleton<Database>();

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IApplicationDbContext, ApplicationDbContext>();
            services.AddSingleton<IApplicationQueryDbContext, ApplicationQueryDbContext>();

            services.AddTransient<IUserRepositoryAsync, UserRepositoryAsync>();
            services.AddTransient<ICustomerRepositoryAsync, CustomerRepositoryAsync>();
            services.AddTransient<IProductGroupRepositoryAsync, ProductGroupRepositoryAsync>();
            services.AddTransient<IOriginRepositoryAsync, OriginRepositoryAsync>();
            services.AddTransient<IProductCodeRepositoryAsync, ProductCodeRepositoryAsync>();
            services.AddTransient<IProductRepositoryAsync, ProductRepositoryAsync>();
            services.AddTransient<IWarehouseRepositoryAsync, WarehouseRepositoryAsync>();
            services.AddTransient<ICounterRepositoryAsync, CounterRepositoryAsync>();
            services.AddTransient<IInvoiceLineRepositoryAsync, InvoiceLineRepositoryAsync>();
            services.AddTransient<IInvoiceRepositoryAsync, InvoiceRepositoryAsync>();
            services.AddTransient<IVatRateRepositoryAsync, VatRateRepositoryAsync>();
            services.AddTransient<IOfferRepositoryAsync, OfferRepositoryAsync>();
            services.AddTransient<IOfferLineRepositoryAsync, OfferLineRepositoryAsync>();
            services.AddTransient<IStockRepositoryAsync, StockRepositoryAsync>();
            services.AddTransient<IStockCardRepositoryAsync, StockCardRepositoryAsync>();
            services.AddTransient<IInvCtrlPeriodRepositoryAsync, InvCtrlPeriodRepositoryAsync>();
            services.AddTransient<IInvCtrlRepositoryAsync, InvCtrlRepositoryAsync>();
            services.AddTransient<ICustDiscountRepositoryAsync, CustDiscountRepositoryAsync>();
            services.AddTransient<IZipRepositoryAsync, ZipRepositoryAsync>();
            services.AddTransient<ILocationRepositoryAsync, LocationRepositoryAsync>();
            services.AddTransient<IWhsTransferRepositoryAsync, WhsTransferRepositoryAsync>();


            services.AddSingleton<ICacheService<Product>, ProductCacheService>();
            services.AddSingleton<ICacheService<Customer>, CustomerCacheService>();
            services.AddSingleton<ICacheService<ProductGroup>, ProductGroupCacheService>();
            services.AddSingleton<ICacheService<Origin>, OriginCacheService>();
            services.AddSingleton<ICacheService<VatRate>, VatRateCacheService>();

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
                .WithGlobalCommandTimeout(TimeSpan.FromMinutes(30))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

        }
    }
}