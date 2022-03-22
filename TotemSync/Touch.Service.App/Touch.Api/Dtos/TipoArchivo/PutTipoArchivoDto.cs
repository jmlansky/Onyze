using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.TipoArchivo
{
    public class PutTipoArchivoDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(3, ErrorMessage = "El nombre tiene que tener mas de 4 caracteres")]
        public string Nombre { get; set; }
    }
}
