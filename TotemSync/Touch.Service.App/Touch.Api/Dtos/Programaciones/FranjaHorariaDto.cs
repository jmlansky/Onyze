using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Programaciones
{
    public class FranjaHorariaDto
    {
        [JsonProperty("horaDesde")]
        public string HoraDesde { get; set; }

        [JsonProperty("horaHasta")]
        public string HoraHasta { get; set; }

    }
}
