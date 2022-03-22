using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Framework.Auth
{
    public static class ActiveUsers
    {
        public static Dictionary<string, string> SesionesActivas { get; set; } = new Dictionary<string, string>();
    }
}
