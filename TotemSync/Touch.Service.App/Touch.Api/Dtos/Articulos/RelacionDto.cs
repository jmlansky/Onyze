using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Articulos
{
    public class RelacionDto:ComunDto
    {
        [JsonProperty("idOrigen")]
        public long IdOrigen { get; set; }

        [JsonProperty("idDestino")]
        public long IdDestino { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

    }
}
