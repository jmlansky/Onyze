using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Sectores;

namespace Touch.Api.Dtos.Playlists
{
    public class PutPlaylistsDto
    {
        [JsonProperty("idCliente", Required = Required.Always)]
        public long IdCliente { get; set; }

        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4, ErrorMessage = "Por favor ingrese un nombre mas largo.")]
        public string Nombre { get; set; }

        [JsonProperty("tiempoPredeterminado", Required = Required.Always)]
        public int TiempoPredeterminado { get; set; }


        [JsonProperty("multimedia")]
        public List<PutMultimediaDto> Multimedia { get; set; } = new List<PutMultimediaDto>();

        [JsonProperty("sectores")]
        public List<PlaylistDeSectorDto> Sectores { get; set; } = new List<PlaylistDeSectorDto>();

        [JsonProperty("activo", Required = Required.Always)]
        public bool Activo { get; set; }
    }
}
