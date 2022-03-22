using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.ArticulosMultiples
{
    public class DeleteArticulosMultiplesDto
    {
        [JsonProperty("idOrigen", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id origen mayor que {1}")]
        public long IdOrigen { get; set; }

        [JsonProperty("idDestino", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id destino mayor que {1}")]
        public long IdDestino { get; set; }
    }
}
