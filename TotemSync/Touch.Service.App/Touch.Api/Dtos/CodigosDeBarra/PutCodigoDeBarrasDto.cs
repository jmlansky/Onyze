using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.CodigosDeBarra
{
    public class PutCodigoDeBarrasDto
    {
        [JsonProperty("idArticulo", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage = "Por favor ingrese un id de artículo mayor a {1}.")]
        public long IdArticulo { get; set; }

        [JsonProperty("ean", Required = Required.Always)]
        [MinLength(4)]
        [MaxLength(100)]
        public string EAN { get; set; }
    }
}
