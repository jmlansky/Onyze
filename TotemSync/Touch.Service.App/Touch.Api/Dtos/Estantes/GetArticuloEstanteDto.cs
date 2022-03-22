using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Touch.Api.Dtos.CodigosDeBarra;

namespace Touch.Api.Dtos.Estantes
{
    public class GetArticuloEstanteDto
    {
        [JsonProperty("articulos")]
        public List<ArticuloEstanteDto> Articulos { get; set; }

        [JsonProperty("codigosDeBarra")]
        public IEnumerable<CodigoDeBarrasDto> CodigosDeBarra { get; set; }
    }
}
