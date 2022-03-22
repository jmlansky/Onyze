using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Framework.Helpers
{
    public static class InstancesHelper
    {
        public static T GetInstanciaAplicar<T>(string name)
        {
            var type = typeof(T);
            var selected = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .FirstOrDefault(x => x.Name.Equals(name));

            if (selected != null)
                return (T)Activator.CreateInstance(selected);
            return default;
        }

        public static T GetImplementation<T>(string attributeName, IConfiguration configuration)
        {
            var type = typeof(T);
            var assamblies = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));


            foreach (var implementation in assamblies)
            {
                if (!implementation.GetCustomAttributesData().Any())
                    continue;
                var implementationAttributeName = implementation.GetCustomAttributesData().FirstOrDefault().ConstructorArguments.FirstOrDefault().Value;
                if (implementationAttributeName.Equals(attributeName))
                    return (T)Activator.CreateInstance(implementation, configuration);
            }

            return default;
        }
    }
}
