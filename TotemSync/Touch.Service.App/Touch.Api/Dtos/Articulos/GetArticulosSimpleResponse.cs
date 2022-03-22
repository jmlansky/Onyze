using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Archivos;

namespace Touch.Api.Dtos.Articulos
{
    public class GetArticulosSimpleResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; }

        [JsonProperty("descripcionLarga")]
        public string DescripcionLarga { get; set; }

        [JsonProperty("etiquetas")]
        public string Etiquetas { get; set; }

        [JsonProperty("sku")]
        public string SKU { get; set; }

        [JsonProperty("precio")]
        public decimal Precio { get; set; }

        [JsonProperty("prospecto")]
        public string Prospecto { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("idFabricante")]
        public long IdFabricante { get; set; }

        [JsonProperty("idTipo")]
        public long IdTipo { get; set; }

        [JsonProperty("archivos")]
        public List<ArchivoDto> Archivos { get; set; }

        [JsonProperty("codigos")]
        public List<string> Codigos { get; set; } = new List<string>();
    }
}
