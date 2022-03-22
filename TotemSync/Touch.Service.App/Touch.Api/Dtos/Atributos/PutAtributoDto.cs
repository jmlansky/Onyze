using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Atributos
{
    public class PutAtributoDto
    {
        [JsonProperty("idTipo", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id origen mayor que {1}")]
        public long IdTipo { get; set; }

        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }
    }
}
