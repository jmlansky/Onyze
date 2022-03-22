using Newtonsoft.Json;
using System.Collections.Generic;

namespace Touch.Api.Dtos.Programaciones
{
    public class ItemDeProgramacionDto
    {
        [JsonProperty("idsPlaylist")]
        public List<long> IdsPlaylist { get; set; } = new List<long>();

        [JsonProperty("idsSponsoreo")]
        public List<long> IdsSponsoreo { get; set; } = new List<long>();
    }
}
