using Newtonsoft.Json;

namespace Totem.Sync.Api.Dtos
{
    public class ItemsProgramadosDto: DtoBase
    {
        [JsonProperty("tipo")]
        public string Tipo { get; set; }
    }
}