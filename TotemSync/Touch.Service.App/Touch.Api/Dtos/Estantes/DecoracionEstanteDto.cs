using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Archivos;

namespace Touch.Api.Dtos.Estantes
{
    public class DecoracionEstanteDto
    {
        [JsonProperty("idEstante")]
        public long IdEstante { get; set; }

        [JsonProperty("texto")]
        public string Texto { get; set; }

        [JsonProperty("ubicacion")]
        public string Ubicacion { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("archivo")]
        public ArchivoDto Archivo { get; set; } 

        [JsonProperty("ancho")]
        public long Ancho { get; set; }

        [JsonProperty("alto")]
        public long Alto { get; set; }

        [JsonProperty("posicionInicial")]
        public long PosicionInicial { get; set; }

        [JsonProperty("desplazamientoX")]
        public long DesplazamientoX { get; set; }

        [JsonProperty("desplazamientoY")]
        public long DesplazamientoY { get; set; }
    }
}
