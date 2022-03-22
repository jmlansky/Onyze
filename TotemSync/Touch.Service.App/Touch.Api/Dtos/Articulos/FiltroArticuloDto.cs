using Newtonsoft.Json;
using System.Collections.Generic;
using Touch.Api.Dtos.CategoriasDeArticulo;

namespace Touch.Api.Dtos.Articulos
{
    public class FiltroArticuloDto: ComunDto
    {
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("sku")]
        public string SKU { get; set; }

        [JsonProperty("atributos")]
        public IEnumerable<AtributoDto> Atributos { get; set; }

        [JsonProperty("fabricante")]
        public FabricanteDto Fabricante{ get; set; }

        [JsonProperty("categoria")]
        public CategoriaDto Categoria { get; set; }

        [JsonProperty("tipoArticulo")]
        public TipoArticuloDto TipoArticulo { get; set; }
    }
}
