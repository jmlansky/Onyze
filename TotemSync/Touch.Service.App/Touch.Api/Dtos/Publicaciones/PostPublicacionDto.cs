using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Publicaciones
{
    public class PostPublicacionDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("idArchivo")]
        public long IdArchivo { get; set; }

        [JsonProperty("idCliente")]
        public long IdCliente { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("objetosAPublicitar")]
        public List<PostObjetoAPublicitarDto> ObjetosAPublicitar{ get; set; }       
    }
}
