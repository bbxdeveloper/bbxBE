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


namespace bbxBE.Application.Queries
{
    public static class QueryServiceRegistration
    {
        public static void AddQueryInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); //Validáció behúzása?

            services.AddScoped<IDataShapeHelper<USR_USER>, DataShapeHelper<USR_USER>>();    // másutt is létre van hozva. Ez kell ide?
            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();
            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();



            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("Handler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
        }



    }
}
