using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.TiposAtributo
{
    public class PostTipoAtributoDto
    {
        [JsonProperty("nombre", Required =Required.Always)]
        [MinLength(3, ErrorMessage = "El nombre tiene que tener más de {1} caracteres")]
        public string Nombre { get; set; }
    }
}
