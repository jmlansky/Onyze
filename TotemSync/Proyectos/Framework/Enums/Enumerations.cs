using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Enums
{
    public static class Enumerations
    {

        public enum ServiceMethod 
        {
            Insert,
            Update,
            Delete
        }

        public enum ServiceMethodsStatusCode
        {
            Ok = 200,
            PartialContent = 206,
            Error = 400
        }
    }
}
