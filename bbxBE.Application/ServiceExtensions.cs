using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Application.Queries.ViewModels;
using bbxBE.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace bbxBE.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
      
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); //Validáció behúzása?

            services.AddMediatR(Assembly.GetExecutingAssembly());                       //Controller  Mediator DI -hez

  
            services.AddScoped<IDataShapeHelper<USR_USER>, DataShapeHelper<USR_USER>>();

            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();
            services.AddScoped<IDataShapeHelper<GetCustomerViewModel>, DataShapeHelper<GetCustomerViewModel>>();


            services.AddScoped<IDataShapeHelper<ProductGroup>, DataShapeHelper<ProductGroup>>();
            services.AddScoped<IDataShapeHelper<GetProductGroupViewModel>, DataShapeHelper<GetProductGroupViewModel>>();
  

            services.AddScoped<IModelHelper, ModelHelper>();
            //services.AddScoped<IMockData, MockData>();
        }
    }
}