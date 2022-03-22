using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Articulos
{
    public class PutAtributosDelArticuloDto
    {
        [JsonProperty("idArticulos")]
        public IEnumerable<long> IdArticulos { get; set; }
    }
}
