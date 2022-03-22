using Newtonsoft.Json;
using System.Collections.Generic;

namespace Totem.Sync.Api.Dtos
{
    public class PlaylistDto : DtoBase
    {
        [JsonProperty("tiempoDeterminado")]
        public int TiempoDeterminado { get; set; }

        [JsonProperty("programaciones")]
        public List<ProgramacionDto> Programaciones { get; set; }
    }
}