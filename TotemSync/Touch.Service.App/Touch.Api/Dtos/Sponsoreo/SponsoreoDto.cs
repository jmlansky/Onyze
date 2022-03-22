using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Sponsoreo
{
    public class SponsoreoDto : ComunDto
    {
        [JsonProperty("fechaInicio")]
        public string FechaInicio { get; set; }

        [JsonProperty("fechaInicio")]
        public string FechaFin { get; set; }

        [JsonProperty("fechaFin")]
        public string HoraInicio { get; set; }

        [JsonProperty("horaInicio")]
        public string HoraFin { get; set; }

        [JsonProperty("idArticulo")]
        public long IdArticulo { get; set; }

        [JsonProperty("idFabricante")]
        public long IdFabricante { get; set; }
    }
}
