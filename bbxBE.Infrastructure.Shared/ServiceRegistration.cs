using bbxBE.Application.Interfaces;
using bbxBE.Domain.Settings;
using bbxBE.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace bbxBE.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.Configure<JWTSettings>(_config.GetSection("JWTSettings"));
            services.Configure<NAVSettings>(_config.GetSection("NAVSettings"));
            services.Configure<ProductCacheSettings>(_config.GetSection("ProductCacheSettings"));

            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<IMockService, MockService>();

        }
    }
}