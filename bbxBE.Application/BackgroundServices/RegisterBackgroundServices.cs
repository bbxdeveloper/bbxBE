using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace bbxBE.Application.BackgroundServices
{

    public static class RegisterBackgroundServices
    {
        /*     public static void AddHostedServices(this IServiceCollection services, IConfiguration configuration)
             {


                 Assembly.GetExecutingAssembly().GetTypes().Where(w => w.Name.EndsWith("BackgroundService")).ToList().ForEach((t) =>
                 {
                     services.AddHostedService(t.GetTypeInfo().ImplementedInterfaces.First(), t);
                 });
             }
        */


        /// <summary>
        /// Adds the background services under specific namespace.
        /// </summary>
        /// <param name="services">The instance IServiceCollection.</param>
        /// <param name="backgroundServiceNamespace">The namespace of background service.</param>
        /// <returns>The configured service.</returns>
        public static void AddHostedServices(
                this IServiceCollection services, IConfiguration configuration,
                string? backgroundServiceNamespace = null)
        {
            // Gets method info from static extensions class where ddHostedService<T> is defined.
            var addHostedService = typeof(ServiceCollectionHostedServiceExtensions).GetMethod(
                // To indicate the name of the method.
                name: nameof(ServiceCollectionHostedServiceExtensions.AddHostedService),
                // To indicate the target method has one generic argument.
                genericParameterCount: 1,
                // To indicate the target method is static and public.
                bindingAttr: BindingFlags.Static |
                             BindingFlags.Public,
                // To indicate using the default binder.
                binder: null,
                // To indicate the target method takes IServiceCollection as parameter.
                types: new[] { typeof(IServiceCollection) },
                modifiers: null);

            // Gets all background services under the namespace (if specified).
            var backgroundServiceTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(type =>
                    type is { IsClass: true, IsAbstract: false } &&
                    IsMicrosoftExtensionsHostingBackgroundService(type) &&
                    (string.IsNullOrWhiteSpace(backgroundServiceNamespace) ||
                     backgroundServiceNamespace.Equals(type.Namespace)))
                .ToArray();

            foreach (var backgroundServiceType in backgroundServiceTypes)
            {
                // The first parameter is null since it is a static method.
                addHostedService!.MakeGenericMethod(backgroundServiceType)
                    .Invoke(null, new object?[] { services });
            }
        }

        /// <summary>
        /// Checks whether the type is Microsoft.Extensions.Hosting.BackgroundService.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True means current class inherit from Microsoft.Extensions.Hosting.BackgroundService.</returns>
        private static bool IsMicrosoftExtensionsHostingBackgroundService(Type type)
        {
            var backgroundServiceFullName = typeof(Microsoft.Extensions.Hosting.BackgroundService).FullName!;
            var baseType = type.BaseType;

            while (baseType != null)
            {
                if (backgroundServiceFullName.Equals(baseType.FullName))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}
