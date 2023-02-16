using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace AhaTech.Cqs.AspnetCore
{
    public static class CqsServiceExtensions
    {
        /// <summary>
        /// Autodiscovers CQS command and query handlers and registers them, along with the CQS infrastructure.
        /// CQS actions will be discoverable under /Api/[CQS action] where CQS action is the fully-qualified name of the
        /// command or query with the namespace of the cqsAssembly removed, and '.' replaced with '/' 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="cqsAssembly">The assembly containing the DTOs implementing ICommand or IQuery (can also contain the related handlers)</param>
        /// <param name="baseNamespace">Overrides the namespace to remove from the commands and queries (defaults to the name of the cqsAssembly)</param>
        /// <param name="extraHandlerAssemblies">Extra assemblies for command- or query handlers</param>
        public static void AddCqs(
            this IServiceCollection services,
            Assembly cqsAssembly,
            string? baseNamespace = null,
            params Assembly[] extraHandlerAssemblies)
        {
            var types = cqsAssembly.GetTypes();
            var assemblyNamespace = baseNamespace ?? cqsAssembly.GetName().Name!;
            var commands = types.Where(t => t.IsAssignableTo(typeof(ICommand))).ToList();
            var commandsWithResult = types.Where(t => t.IsAssignableToGeneric(typeof(ICommand<>))).ToList();
            var queries = types.Where(t => t.IsAssignableToGeneric(typeof(IQuery<>))).ToList();

            services.AddControllers(conf => conf.Conventions.Add(new CqsControllerConvention(assemblyNamespace)))
                .ConfigureApplicationPartManager(conf =>
                    conf.FeatureProviders.Add(new CqsControllerFeatureProvider(commands, commandsWithResult, queries)));

            var handlers = extraHandlerAssemblies
                .Append(cqsAssembly)
                .SelectMany(ass => ass.GetTypes())
                .Select(GetCqsHandlerTypes)
                .Where(t => t.HasValue)
                .Select(t => t!.Value);

            foreach (var handler in handlers)
            {
                services.AddScoped(handler.Interface, handler.Type);
            }
        }

        public static void MapCqs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        private static (Type Type, Type Interface)? GetCqsHandlerTypes(Type type)
        {
            foreach (var iFace in type.GetInterfaces().Where(t => t.IsGenericType))
            {
                var genericInterface = iFace.GetGenericTypeDefinition();
                if (genericInterface == typeof(ICommandHandler<>) ||
                    genericInterface == typeof(ICommandHandler<,>) ||
                    genericInterface == typeof(IQueryHandler<,>))
                {
                    return (type, iFace);
                }
            }

            return null;
        }

        private static bool IsAssignableToGeneric(this Type givenType, Type genericType)
        {
            // Based on https://stackoverflow.com/a/1075059

            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGeneric(baseType, genericType);
        }
    }
}