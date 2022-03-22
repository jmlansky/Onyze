using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Localidades
{
    public class PutLocalidadDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4)]
        public string Nombre { get; set; }

        [JsonProperty("idProvincia", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage = "Por favor ingrese un id de provincia mayor que {1}")]
        public long IdProvincia { get; set; }
    }
}
