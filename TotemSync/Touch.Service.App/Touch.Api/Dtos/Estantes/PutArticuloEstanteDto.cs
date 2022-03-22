using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Estantes
{
    public class PutArticuloEstanteDto
    {
        [JsonProperty("articulos")]
        public List<ArticuloEstanteDto> Articulos { get; set; }
    }
}
