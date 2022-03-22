using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Promociones
{
    public class FiltrosPromocionesDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("idTipo")]
        public long IdTipo { get; set; }

        [JsonProperty("destinatarios")]
        public DestinatariosDePromoDto Destinatarios { get; set; }

        [JsonProperty("articulos")]
        public List<DetallePromocionDto> Articulos { get; set; }
    }
}
