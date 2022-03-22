using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Articulos;

namespace Touch.Api.Dtos.CategoriasDeArticulo
{
    public class PostCategoriaDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4)]
        [MaxLength(30)]
        public string Nombre { get; set; }

        [JsonProperty("idCategoriaPadre")]
        public long? IdCategoriaPadre { get; set; }

        [JsonProperty("subcategorias")]
        public List<PostCategoriaDto> Subcategorias { get; set; }

        [JsonProperty("idArchivo")]
        public long? IdArchivo { get; set; }

        [JsonProperty("mostarEnTotem")]
        public bool MostrarEnTotem { get; set; }
    }
}
