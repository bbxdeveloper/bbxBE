using bbxBE.Application.Commands.cmdImport;
using bbxBE.Application.Interfaces.Commands;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace bbxBE.Application.Commands
{
    public static class CommandServiceRegistration
    {
        public static void AddCommandInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("CommandHandler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });


            services.AddTransient<IImportProductProc, ImportProductProc>();

        }



    }
}
