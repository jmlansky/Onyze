using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class DetallePromocionDto
    {
        [JsonProperty("idArticulo")]        
        public long IdArticulo { get; set; }

        [JsonProperty("precioAnterior")]
        public decimal PrecioAnterior { get; set; }
    }
}
