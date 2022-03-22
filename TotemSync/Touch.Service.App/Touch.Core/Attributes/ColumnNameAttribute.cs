using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    
    public class ColumnNameAttribute: Attribute
    {
        public ColumnNameAttribute(string name)
        {
            ColumnName = name;            
        }

        public string ColumnName { get; }
    }
}
