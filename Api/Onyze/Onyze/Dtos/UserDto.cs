using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Onyze.Dtos
{
    public class UserDto
    {
        public string Dni { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public int Age { get; set; }
    }
}
