using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.CategoriasDeArticulo;

namespace Touch.Api.Dtos.Promociones
{
    public class ItemsDePromocionDto
    {
        public List<IItemDePromocionDto> Items{ get; set; }

        [JsonProperty("detalles")]
        public List<GetDetallePromocionDto> Detalles { get; set; }

        [JsonProperty("categorias")]
        public List<GetPromocionCategoriaDto> Categorias{ get; set; }

        [JsonProperty("marcas")]
        public List<GetMarcasDePromocionDto> Marcas { get; set; }

    }
}
