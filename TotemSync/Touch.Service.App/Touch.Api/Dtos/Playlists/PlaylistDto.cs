using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Publicaciones;
using Touch.Api.Dtos.Sectores;

namespace Touch.Api.Dtos.Playlists
{
    public class PlaylistDto: ComunDto
    {
        [JsonProperty("idCliente")]
        public long IdCliente { get; set; }

        [JsonProperty("tiempoPredeterminado")]
        public int TiempoPredeterminado { get; set; }

        
        [JsonProperty("multimedia")]
        public List<MultimediaDto> Multimedia { get; set; } = new List<MultimediaDto>();


        [JsonProperty("sectores")]
        public List<SectorDto> Sectores { get; set; } = new List<SectorDto>();

        [JsonProperty("activo", Required = Required.Always)]
        public bool Activo { get; set; }
    }
}
