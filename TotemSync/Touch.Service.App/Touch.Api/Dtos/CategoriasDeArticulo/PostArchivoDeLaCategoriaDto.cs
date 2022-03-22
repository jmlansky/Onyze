using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.CategoriasDeArticulo
{
    public class PostArchivoDeLaCategoriaDto
    {
        [JsonProperty("idArchivo", Required = Required.Always)]
        [Range(1, long.MaxValue, ErrorMessage ="Por favor ingrese un id válido")]
        public long IdArchivo { get; set; }
    }
}
