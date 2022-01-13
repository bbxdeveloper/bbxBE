using bbxBE.Application.Interfaces;
using bbxBE.Application.Interfaces.Repositories;
using bbxBE.Infrastructure.Persistence.Contexts;
using bbxBE.Infrastructure.Persistence.Repositories;
using bbxBE.Infrastructure.Persistence.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq;
using bbxBE.Domain.Extensions;
using MediatR;

namespace bbxBE.Commands
{
    public static class ServiceRegistration
    {
        public static void AddCommandInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<DapperContext>();

            #region Commands

            Assembly.GetEntryAssembly().GetTypes().Where(w=>w.Name.EndsWith("CommandHandler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
            /*
            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IRequestHandler>().ForEach((t) =>
            {
                services.AddTransient(t, t);
            });
            */

            
            #endregion Commands
        }

    }
}
