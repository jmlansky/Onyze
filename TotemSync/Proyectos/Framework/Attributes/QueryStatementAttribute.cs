using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Attributes
{
    [AttributeUsage(AttributeTargets.All)]

    public class QueryStatementAttribute : Attribute
    {
        public QueryStatementAttribute(string name)
        {
            QueryStatementName = name;
        }

        public string QueryStatementName { get; }
    }
}
