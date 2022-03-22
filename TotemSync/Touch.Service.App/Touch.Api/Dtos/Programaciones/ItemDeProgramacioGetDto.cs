using Newtonsoft.Json;
using System.Collections.Generic;

namespace Touch.Api.Dtos.Programaciones
{
    public class ItemDeProgramacioGetDto
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("tipo")]
        public string Tipo { get; set; }
    }
}
