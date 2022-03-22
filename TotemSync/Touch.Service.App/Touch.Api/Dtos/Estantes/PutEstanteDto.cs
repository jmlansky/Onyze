using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Estantes
{
    public class PutEstanteDto
    {
        [JsonProperty("color")]        
        public string Color { get; set; }

        [JsonProperty("altura")]
        public decimal Altura { get; set; }

        [JsonProperty("alto")]
        public decimal Alto { get; set; }

        [JsonProperty("alturaContenedorPx")]
        public decimal AlturaContenedorPx { get; set; }

        [JsonProperty("orden")]
        public long Orden { get; set; }        

        [JsonProperty("decoraciones")]
        public List<PostDecoracionEstanteDto> Decoraciones { get; set; } = new List<PostDecoracionEstanteDto>();

        [JsonProperty("articulos")]
        public List<PostArticuloEstanteDto> Articulos { get; set; } = new List<PostArticuloEstanteDto>();
    }
}
