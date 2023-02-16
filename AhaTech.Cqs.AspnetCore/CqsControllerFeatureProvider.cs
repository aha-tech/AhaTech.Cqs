using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace AhaTech.Cqs.AspnetCore
{
    internal class CqsControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly List<Type> _commands;
        private readonly List<Type> _commandsWithResult;
        private readonly List<Type> _queries;

        public CqsControllerFeatureProvider(List<Type> commands, List<Type> commandsWithResult, List<Type> queries)
        {
            _commands = commands;
            _commandsWithResult = commandsWithResult;
            _queries = queries;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var command in _commands)
            {
                feature.Controllers.Add(typeof(CommandController<>).MakeGenericType(command).GetTypeInfo());
            }
            foreach (var command in _commandsWithResult)
            {
                var result = GetInterfaceResult(command, typeof(ICommand<>));
                feature.Controllers.Add(typeof(CommandController<,>).MakeGenericType(command, result).GetTypeInfo());
            }
            foreach (var query in _queries)
            {
                var result = GetInterfaceResult(query, typeof(IQuery<>));
                feature.Controllers.Add(typeof(QueryController<,>).MakeGenericType(query, result).GetTypeInfo());
            }
        }

        private Type GetInterfaceResult(Type type, Type genericInterface)
        {
            return type.GetInterfaces()
                .Single(t => t.IsGenericType && t.GetGenericTypeDefinition() == genericInterface)
                .GetGenericArguments()
                .Single();
        }
    }
}