using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

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

            //var assamblies = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Any(z => z.Name.Equals(typeof(T).Name))).ToList().Where(x => x.IsAssignableFrom(x));

            foreach (var implementation in assamblies)
            {
                if (!implementation.GetCustomAttributesData().Any())
                    continue;
                var implementationAttributeName = implementation.GetCustomAttributesData().FirstOrDefault().ConstructorArguments.FirstOrDefault().Value;
                if (implementationAttributeName.Equals(attributeName))
                {
                    var t = implementation.MakeGenericType(typeof(T));
                    dynamic configurationInstance = Activator.CreateInstance(t);

                    return (T)Activator.CreateInstance(implementation, configuration);
                }

            }

            return default;
        }
    }
}
