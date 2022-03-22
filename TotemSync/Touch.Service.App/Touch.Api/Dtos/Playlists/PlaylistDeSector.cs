using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Publicaciones;
using Touch.Api.Dtos.Sectores;

namespace Touch.Api.Dtos.Playlists
{
    public class PlaylistDeSectorDto  
    {
        [JsonProperty("id_sector")]
        public long IdSector { get; set; }

    }
}
