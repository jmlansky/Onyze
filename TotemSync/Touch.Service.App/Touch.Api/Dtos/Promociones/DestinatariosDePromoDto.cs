using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class DestinatariosDePromoDto
    {
        [JsonProperty("idsCategoria")]
        public List<long> IdsGrupo { get; set; }

        [JsonProperty("idsProvincia")]
        public List<long> IdsProvincia { get; set; }

        [JsonProperty("idsRegion")]
        public List<long> IdsRegion { get; set; }

        [JsonProperty("idsCliente")]
        public List<long> IdsCliente { get; set; }
    }
}
