using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AhaTech.Cqs.AspnetCore
{
    internal class CqsControllerConvention : IControllerModelConvention
    {
        private readonly string _baseNamespace;

        public CqsControllerConvention(string baseNamespace)
        {
            _baseNamespace = baseNamespace;
        }
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType)
            {
                return;
            }

            var genericControllerType = controller.ControllerType.GetGenericTypeDefinition();
            
            if (genericControllerType == typeof(CommandController<>) || 
                genericControllerType == typeof(CommandController<,>) || 
                genericControllerType == typeof(QueryController<,>))
            {
                var typeArguments = controller.ControllerType.GetGenericArguments();
                var route = GetRoute(typeArguments[0]);
                controller.Selectors.Single().AttributeRouteModel!.Template = route;
            }
        }

        private string GetRoute(Type cqsType)
        {
            // Since we're only removing the namespace, the cqsName will start with a '/'
            var cqsName = cqsType.FullName![_baseNamespace.Length..].Replace('.', '/');
            return $"/Api{cqsName}";
        }
    }
}