using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Attributes
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
