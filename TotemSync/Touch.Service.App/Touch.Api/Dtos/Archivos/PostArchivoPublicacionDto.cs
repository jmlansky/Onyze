using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Archivos
{
    public class PostArchivoPublicacionDto
    {
        [JsonProperty("idPublicacion")]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id de artículo tipo mayor que {1}.")]
        public long IdPublicacion { get; set; }

        [JsonProperty("idTipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id de tipo mayor que {1}.")]
        public long IdTipo { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("small")]
        public bool Small { get; set; }

        [JsonProperty("medium")]
        public bool Medium { get; set; }

        [JsonProperty("large")]
        public bool Large { get; set; }

        [JsonProperty("file", Required = Required.Always)]
        public IFormFile File { get; set; }
    }
}
