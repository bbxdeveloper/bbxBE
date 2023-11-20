using bbxBE.Application.Behaviours;
using bbxBE.Application.Helpers;
using bbxBE.Application.Interfaces;
using bbxBE.Common.Attributes;
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

            FluentValidation.ValidatorOptions.Global.DisplayNameResolver = (type, member, expression) =>
            {
                if (member != null)
                {
                    var labelAttr = member.GetCustomAttribute<ColumnLabelAttribute>();
                    if (labelAttr != null)
                        return member.GetCustomAttribute<ColumnLabelAttribute>()?.LabelText;
                    else
                        return "?";
                }
                return "?";
            };


            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>)); //Validáció behúzása?

            services.AddMediatR(Assembly.GetExecutingAssembly());                       //Controller  Mediator DI -hez

            services.AddSingleton<IModelHelper, ModelHelper>();

            //services.AddScoped<IMockData, MockData>();
        }
    }
}