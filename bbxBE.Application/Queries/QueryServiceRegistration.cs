using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Queries.ViewModels;
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

       
            services.AddScoped<IDataShapeHelper<USR_USER>, DataShapeHelper<USR_USER>>();

            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();
            services.AddScoped<IDataShapeHelper<GetCustomerViewModel>, DataShapeHelper<GetCustomerViewModel>>();


            services.AddScoped<IDataShapeHelper<ProductGroup>, DataShapeHelper<ProductGroup>>();
            services.AddScoped<IDataShapeHelper<GetProductGroupViewModel>, DataShapeHelper<GetProductGroupViewModel>>();

            services.AddScoped<IDataShapeHelper<Origin>, DataShapeHelper<Origin>>();
            services.AddScoped<IDataShapeHelper<GetOriginViewModel>, DataShapeHelper<GetOriginViewModel>>();

            services.AddScoped<IDataShapeHelper<Product>, DataShapeHelper<Product>>();
            services.AddScoped<IDataShapeHelper<GetProductViewModel>, DataShapeHelper<GetProductViewModel>>();

            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("Handler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
        }



    }
}
