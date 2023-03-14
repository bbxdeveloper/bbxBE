using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using System;
using AsyncKeyedLock;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace bbxBE.Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddCommonInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
                // configuration.GetValue<bool>("UseInMemoryDatabase") .. ha konfigból kell felolvasni valamit


                //https://github.com/MarkCiliaVincenti/AsyncKeyedLock/wiki/How-to-use-AsyncKeyedLocker
                //https://stackoverflow.com/questions/34834295/dependency-injection-inject-with-parameters
                services.AddSingleton<AsyncKeyedLocker<string>>(
                provider => new AsyncKeyedLocker<string>( new AsyncKeyedLockOptions( 10, 100, 1)));

           
        }
    }
}