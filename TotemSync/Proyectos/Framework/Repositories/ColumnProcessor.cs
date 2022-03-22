using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.Repositories
{
    public static class ColumnProcessor<T>
    {
        public static string GetColumnsForSelect(string alias, string[] columnsToIgnore)
        {            
            var columns = new List<string>();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null && columnsToIgnore.Any() && NeedToIgnore(prop, columnsToIgnore))
                    continue;

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null)
                    columns.Add(alias + "." + column.ColumnName + " as " + prop.Name);
                else
                    columns.Add(alias + "." + prop.Name);
            }
            var result = string.Join(", ", columns);
            return result;
        }

        private static bool NeedToIgnore(PropertyInfo prop, string[] columnsToIgnore)
        {
            if (columnsToIgnore != null)
            {
                if (columnsToIgnore.Contains(prop.Name))
                    return true;
            }

            if (!prop.PropertyType.Namespace.Equals("System"))
                return true;

            return false;
        }
    }
}
