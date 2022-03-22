using Newtonsoft.Json;
using System.Collections.Generic;

namespace Totem.Sync.Api.Dtos
{
    public class ProgramacionDto : DtoBase
    {
        [JsonProperty("itemsProgramados")]
        public List<ItemsProgramadosDto> ItemsProgramados { get; set; }

        [JsonProperty("periodos")]
        public List<PeriodoDto> Periodos { get; set; }

        [JsonProperty("activa")]
        public bool Activa { get; set; }
    }
}