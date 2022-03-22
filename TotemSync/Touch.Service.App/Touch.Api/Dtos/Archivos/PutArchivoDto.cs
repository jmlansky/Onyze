using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos
{
    public class PutArchivoDto
    {
        [JsonProperty("idTipo")]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id de tipo mayor que {1}.")]
        public long IdTipo { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("file", Required = Required.AllowNull)]
        public IFormFile File { get; set; }
    }
}
