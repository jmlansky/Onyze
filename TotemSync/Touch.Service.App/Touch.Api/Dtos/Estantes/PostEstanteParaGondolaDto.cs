using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Gondolas;

namespace Touch.Api.Dtos.Estantes
{
    public class PostEstanteParaGondolaDto
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

        [JsonProperty("articulos")]
        public List<PostArticuloEstanteDto> Articulos { get; set; }

        [JsonProperty("decoraciones")]
        public List<PostDecoracionEstanteDto> Decoraciones { get; set; } = new List<PostDecoracionEstanteDto>();
    }
}
