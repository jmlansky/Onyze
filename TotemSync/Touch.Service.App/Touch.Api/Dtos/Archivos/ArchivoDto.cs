using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Archivos
{
    public class ArchivoDto: ComunDto
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }

        [JsonProperty("idArchivoOriginal")]
        public long IdArchivoOriginal { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("miniaturas")]
        public List<ArchivoDto> Miniaturas { get; set; } = new List<ArchivoDto>();
    }
}
