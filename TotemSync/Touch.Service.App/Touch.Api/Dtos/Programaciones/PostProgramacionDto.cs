using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Programaciones
{
    public class PostProgramacionDto : IProgramacionDto
    {

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("items")]
        public ItemDeProgramacionDto Items { get; set; }

        [JsonProperty("periodos")]
        public List<PeriodoDto> Periodos { get; set; } = new List<PeriodoDto>();

        [JsonProperty("destinatarios")]
        public DestinatarioDto Destinatarios { get; set; }

        [JsonProperty("activa")]
        public bool Activa { get; set; }
    }
}
