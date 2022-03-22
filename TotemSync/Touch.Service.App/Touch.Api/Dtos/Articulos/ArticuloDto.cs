using Newtonsoft.Json;
using System.Collections.Generic;
using Touch.Api.Dtos.Archivos;
using Touch.Api.Dtos.CategoriasDeArticulo;
using Touch.Api.Dtos.CodigosDeBarra;

namespace Touch.Api.Dtos.Articulos
{
    public class ArticuloDto : ComunDto
    {        
        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("descripcionLarga")]
        public string DescripcionLarga { get; set; }

        [JsonProperty("etiquetas")]
        public List<string> Etiquetas { get; set; } = new List<string>();

        [JsonProperty("sku")]
        public string SKU { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("prospecto")]
        public string Prospecto { get; set; }

        [JsonProperty("archivos")]
        public List<ArchivoDto> Archivos { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("fabricante")]
        public FabricanteDto Fabricante { get; set; }

        [JsonProperty("atributos")]
        public List<AtributoDto> Atributos { get; set; } = new List<AtributoDto>();

        [JsonProperty("categorias")]
        public List<CategoriaDto> Categorias { get; set; }

        [JsonProperty("tipoArticulo")]
        public TipoArticuloDto TipoArticulo { get; set; }

        [JsonProperty("codigosDeBarra")]
        public IEnumerable<CodigoDeBarrasDto> CodigosDeBarra { get; set; }

        [JsonProperty("idsCruzados")]
        public List<long> IdsCruzados{ get; set; }

        [JsonProperty("idsAlternativos")]
        public List<long> IdsAlternativos { get; set; }
    }
}
