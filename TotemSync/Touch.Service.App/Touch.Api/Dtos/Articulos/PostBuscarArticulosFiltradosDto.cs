using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Atributos;

namespace Touch.Api.Dtos.Articulos
{
    public class PostBuscarArticulosFiltradosDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("sku")]
        public string SKU { get; set; }

        [JsonProperty("atributos")]
        public IEnumerable<AtributosFiltradosDto> Atributos { get; set; }

        [JsonProperty("nombreFabricante")]
        public string NombreFabricante { get; set; }

        [JsonProperty("idFabricante")]
        public long IdFabricante { get; set; }

        [JsonProperty("nombreCategoria")]
        public string NombreCategoria { get; set; }

        [JsonProperty("idCategoria")]
        public long IdCategoria { get; set; }

        [JsonProperty("nombreTipoArticulo")]
        public string TipoArticulo { get; set; }

        [JsonProperty("idTipoArticulo")]
        public long IdTipoArticulo { get; set; }

        [JsonProperty("activo")]
        public bool Activo{ get; set; }
    }
}
