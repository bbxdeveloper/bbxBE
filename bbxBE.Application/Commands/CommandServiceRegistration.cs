using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using FluentValidation;
using MediatR;
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
     
            services.AddScoped<IDataShapeHelper<USR_USER>, DataShapeHelper<USR_USER>>();
            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();
            services.AddScoped<IDataShapeHelper<ProductGroup>, DataShapeHelper<ProductGroup>>();

            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("CommandHandler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
        }



    }
}
