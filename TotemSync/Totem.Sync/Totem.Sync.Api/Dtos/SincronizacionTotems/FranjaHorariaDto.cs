using Newtonsoft.Json;

namespace Totem.Sync.Api.Dtos
{
    public class FranjaHorariaDto
    {
        [JsonProperty("horaDesde")]
        public string HoraDesde { get; set; }

        [JsonProperty("horaHasta")]
        public string HoraHasta { get; set; }
    }
}