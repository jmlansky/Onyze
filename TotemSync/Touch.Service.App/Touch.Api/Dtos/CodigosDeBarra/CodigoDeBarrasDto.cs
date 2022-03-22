using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.CodigosDeBarra
{
    public class CodigoDeBarrasDto: ComunDto
    {
        [JsonProperty("idArticulo")]
        public long IdArticulo { get; set; }

        [JsonProperty("ean")]
        public string EAN { get; set; }
    }
}
