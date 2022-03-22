using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Barrios
{
    public class BarrioDto: ComunDto
    {
        public string CodigoPostal { get; set; }
        public long IdLocalidad { get; set; }
    }
}
