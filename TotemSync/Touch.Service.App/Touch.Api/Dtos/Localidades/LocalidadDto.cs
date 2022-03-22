using Newtonsoft.Json;

namespace Touch.Api.Dtos.Localidades
{
    public class LocalidadDto: ComunDto
    {
        [JsonProperty("idProvincia")]
        public long IdProvincia { get; set; }
    }
}
