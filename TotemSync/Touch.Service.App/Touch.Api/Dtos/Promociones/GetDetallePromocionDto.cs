using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class GetDetallePromocionDto: IGetItemDePromocionDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("nombre")]
        public string Nombre { get; set; }
        
        [JsonProperty("precioAnterior")]
        public decimal PrecioAnterior { get; set; }
        
        [JsonProperty("precioActual")]
        public decimal PrecioActual { get; set; }
    }
}
