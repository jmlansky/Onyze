using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Sectores;

namespace Touch.Api.Dtos.Playlists
{
    public class GetPlaylistsDto:ComunDto
    {
        [JsonProperty("idCliente")]
        public long IdCliente { get; set; }

        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4, ErrorMessage = "Por favor ingrese un nombre mas largo.")]
        public string Nombre { get; set; }

        [JsonProperty("tiempoPredeterminado", Required = Required.Always)]
        public int TiempoPredeterminado { get; set; }


        [JsonProperty("multimedia")]
        public List<GetMultimediaDto> Multimedia { get; set; } = new List<GetMultimediaDto>();


        [JsonProperty("sectores")]
        public List<SectorDto> Sectores { get; set; } = new List<SectorDto>();
      

        [JsonProperty("activo", Required = Required.Always)]
        public bool Activo { get; set; }
    }
}
