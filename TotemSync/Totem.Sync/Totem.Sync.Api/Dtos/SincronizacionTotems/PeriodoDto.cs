using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Totem.Sync.Api.Dtos
{
    public class PeriodoDto: DtoBase
    {
        [JsonProperty("dias")]
        public List<string> Dias { get; set; }

        [JsonProperty("fechaInicio")]
        public DateTime FechaInicio { get; set; }

        [JsonProperty("fechaFin")]
        public DateTime FechaFin { get; set; }

        [JsonProperty("franjasHorarias")]
        public List<FranjaHorariaDto> FranjasHorarias { get; set; }
    }
}