using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Articulos;

namespace Touch.Api.Dtos.CategoriasDeArticulo
{
    public class PutCategoriaDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        public string Nombre { get; set; }

        [JsonProperty("idCategoriaPadre")]
        public long? IdCategoriaPadre { get; set; }

        [JsonProperty("subcategorias")]
        public List<PutCategoriaDto> Subcategorias { get; set; }

        [JsonProperty("idArchivo")]
        public long? IdArchivo { get; set; }

        [JsonProperty("mostarEnTotem")]
        public bool MostrarEnTotem { get; set; }
    }
}
