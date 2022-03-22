using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos
{
    public class ComunDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("creado")]
        public DateTime? Creado { get; set; }

        [JsonProperty("mdificado")]
        public DateTime? Modificado { get; set; }

        [JsonProperty("eliminado")]
        public bool Eliminado { get; set; }
    }
}
