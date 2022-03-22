using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.TiposArticulos
{
    public class PostTipoArticuloDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }
    }
}
