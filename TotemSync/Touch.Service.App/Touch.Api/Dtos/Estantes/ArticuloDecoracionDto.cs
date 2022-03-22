using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Archivos;

namespace Touch.Api.Dtos.Estantes
{
    public class ArticuloDecoracionDto
    {
        [JsonProperty("idArchivo")]
        public long IdArchivo { get; set; }


        [JsonProperty("archivo")]
        public ArchivoDto Archivo { get; set; }

        [JsonProperty("muestraPrecio")]
        public bool MuestraPrecio { get; set; }

        [JsonProperty("posicion")]
        public long Posicion { get; set; }

        [JsonProperty("posicionInicial")]
        public string PosicionInicial  { get; set; }

        [JsonProperty("estiloPrecio")]
        public int EstiloPrecio { get; set; }



        public ArticuloDestacadoDto Destacado { get; set; }

        [JsonProperty("ancho")]
        public long Ancho { get; set; }

        [JsonProperty("alto")]
        public long Alto { get; set; }

        [JsonProperty("desplazamientoX")]
        public long DesplazamientoX { get; set; }

        [JsonProperty("desplazamientoY")]
        public long DesplazamientoY { get; set; }
    }
}
