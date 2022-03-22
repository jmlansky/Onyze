using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Archivos;

namespace Touch.Api.Dtos.CategoriasDeArticulo
{
    public class CategoriaDto : ComunDto
    {
        [JsonProperty("idCategoriaPadre")]
        public long? IdCategoriaPadre { get; set; }

        [JsonProperty("nombreCategoriaPadre")]
        public string NombreCategoriaPadre { get; set; }

        [JsonProperty("subcategorias")]
        public List<CategoriaDto> Subcategorias { get; set; } = new List<CategoriaDto>();

        [JsonProperty("idArchivo")]
        public long? IdArchivo { get; set; }

        [JsonProperty("archivo")]
        public ArchivoDto Archivo { get; set; }

        [JsonProperty("mostarEnTotem")]
        public bool MostrarEnTotem { get; set; }

    }
}
