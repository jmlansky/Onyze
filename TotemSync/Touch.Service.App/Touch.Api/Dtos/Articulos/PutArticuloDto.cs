using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Articulos
{
    public class PutArticuloDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(3)]
        public string Nombre { get; set; }

        [JsonProperty("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [JsonProperty("descripcionLarga")]
        public string DescripcionLarga { get; set; } = string.Empty;

        [JsonProperty("etiquetas")]
        public List<string> Etiquetas { get; set; } = new List<string>();

        [JsonProperty("sku")]
        public string SKU { get; set; }
        
        [JsonProperty("precio")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser un valor numérico positivo")]
        public decimal Precio { get; set; }

        [JsonProperty("prospecto")]
        public string Prospecto { get; set; } = string.Empty;

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("idFabricante", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id de fabricante mayor que {1}")]
        public long IdFabricante { get; set; }

        [JsonProperty("idTipo", Required = Required.Always)]
        [Range(1, int.MaxValue, ErrorMessage = "Por favor ingrese un id de tipo mayor que {1}")]
        public long IdTipo { get; set; }

        [JsonProperty("categoria")]
        public List<long> Categorias { get; set; }

        [JsonProperty("codigos")]
        public List<string> CodigosDeBarra { get; set; } = new List<string>();

        [JsonProperty("atributos")]
        public List<long> Atributos { get; set; } = new List<long>();
    }
}
