using System;
using System.Collections.Generic;
using System.Text;


namespace Touch.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    
    public class ColumnNameAttribute:  Framework.Attributes.ColumnNameAttribute // Attribute
    {

        public ColumnNameAttribute(string name): base(name)
        {
            //ColumnName = name;
        }

        //public string ColumnName { get; }
    }
}
