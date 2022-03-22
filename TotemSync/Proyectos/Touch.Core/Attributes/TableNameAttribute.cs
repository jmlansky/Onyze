using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class TableNameAttribute: Attribute
    {        
        public TableNameAttribute(string name)
        {
            TableName = name;
        }

        public string TableName { get; }
    }
}
