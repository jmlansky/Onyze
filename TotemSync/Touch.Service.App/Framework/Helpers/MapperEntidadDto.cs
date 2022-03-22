using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Helpers
{
    public static class MapperEntidadDto
    {
        public static TResult Mapper<T, TResult>(T entity, TResult result) where TResult : new()
        {
            var ob = Activator.CreateInstance<TResult>();

            foreach (PropertyInfo propertyInfo in typeof(TResult).GetProperties())
            {
                var prop = typeof(T).GetMembers().OfType<PropertyInfo>()
                    .FirstOrDefault(p => p.Name.Equals(propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase) && 
                    p.PropertyType.Name == propertyInfo.PropertyType.Name);

                if (prop!= null && prop.PropertyType.Namespace.Equals("System"))
                    propertyInfo.SetValue(ob, prop.GetValue(entity));
                
            }

            return ob;
        }
    }
}
