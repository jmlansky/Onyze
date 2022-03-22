using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Estantes
{
    public class DeleteArticuloEstanteDto
    {
        [JsonProperty("idsArticulos")]
        public List<long> IdsArticulos{ get; set; }
    }
}
