using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Helpers
{
    public static class CastAndConvert
    {
        public static T CastObject<T>(object input)
        {
            return (T)input;
        }

        public static T ConvertObject<T>(object input)
        {
            return (T)Convert.ChangeType(input, typeof(T));
        }     
    }
}
