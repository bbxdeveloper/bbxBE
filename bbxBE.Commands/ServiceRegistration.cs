﻿using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Domain.Entities;
using bbxBE.Infrastructure.Persistence.Contexts;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace bbxBE.Commands
{
    public static class ServiceRegistration
    {
        public static void AddCommandInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); //Validáció behúzása?
     
            services.AddScoped<IDataShapeHelper<USR_USER>, DataShapeHelper<USR_USER>>();
            services.AddScoped<IDataShapeHelper<Customer>, DataShapeHelper<Customer>>();



            Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("CommandHandler")).ToList().ForEach((t) =>
            {
                services.AddTransient(t.GetTypeInfo().ImplementedInterfaces.First(), t);
            });
        }



    }
}
