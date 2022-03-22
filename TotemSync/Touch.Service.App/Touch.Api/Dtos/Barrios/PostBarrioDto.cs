using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Barrios
{
    public class PostBarrioDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4)]
        public string Nombre { get; set; }

        [JsonProperty("CodigoPostal", Required = Required.Always)]
        [MinLength(4)]
        public string CodigoPostal { get; set; }

        [JsonProperty("IdLocalidad", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage = "Por favor ingrese un id de tipo mayor que {1}")]
        public long IdLocalidad { get; set; }
    }
}
