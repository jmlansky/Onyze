using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Estantes
{
    public class EstanteDto : ComunDto
    {
        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        [JsonProperty("alto")]
        public decimal Alto { get; set; }

        [JsonProperty("orden")]
        public long Orden { get; set; }

        [JsonProperty("alturaContenedorPx")]
        public decimal AlturaContenedorPx { get; set; }

        [JsonProperty("idGondola")]
        public long IdGondola { get; set; }

        [JsonProperty("articulos")]
        public List<ArticuloEstanteDto> Articulos { get; set; } = new List<ArticuloEstanteDto>();

        [JsonProperty("decoraciones")]
        public List<DecoracionEstanteDto> Decoraciones { get; set; } = new List<DecoracionEstanteDto>();

    }
}
