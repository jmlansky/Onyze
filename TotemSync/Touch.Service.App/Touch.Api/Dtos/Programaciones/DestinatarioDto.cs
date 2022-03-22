using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Programaciones
{
    public class DestinatarioDto
    {


        [JsonProperty("idsProvincia")]
        public List<long> IdsProvincia { get; set; }

        [JsonProperty("idsZona")]
        public List<long> IdsZona { get; set; }

        [JsonProperty("idsRegion")]
        public List<long> IdsRegion { get; set; }

        [JsonProperty("idsLocalidad")]
        public List<long> IdsLocalidad { get; set; }

        [JsonProperty("idsCliente")]
        public List<long> IdsCliente { get; set; }


    }
}
