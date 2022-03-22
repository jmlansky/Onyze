using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Articulos
{
    public class PostAtributosDelArticuloDto
    {
        [JsonProperty("idsAtributos")]
        public IEnumerable<long> IdsAtributos { get; set; } = new List<long>();
    }
}
