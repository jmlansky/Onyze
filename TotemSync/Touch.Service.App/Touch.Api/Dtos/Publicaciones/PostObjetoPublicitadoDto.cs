using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Publicaciones
{
    public class PostObjetoPublicitadoDto
    {
        [JsonProperty("idObjeto", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage = "Por favor ingrese un id de objeto mayor que {1}")]
        public long IdObjeto { get; set; }

        [JsonProperty("idTipo", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage = "Por favor ingrese un id de tipo mayor que {1}")]
        public long IdTipo { get; set; }
    }
}
