using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.TiposMultimedia;
using Touch.Api.Dtos.TiposObjetoPublicitar;

namespace Touch.Api.Dtos.Playlists
{
    public class GetMultimediaDto: MultimediaDto
    {
        [JsonProperty("url")]
        public string url { get; set; }

        
    }
}
