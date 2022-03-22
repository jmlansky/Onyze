using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Programaciones
{
    public class PeriodoDto
    {
        [JsonProperty("fechaInicio")]
        public DateTime FechaInicio { get; set; }

        [JsonProperty("fechaFin")]
        public DateTime FechaFin { get; set; }

        [JsonProperty("franjasHorarias")]
        public List<FranjaHorariaDto> FranjasHorarias { get; set; } = new List<FranjaHorariaDto>();

        [JsonProperty("dias")]
        public string Dias { get; set; }
    }
}
