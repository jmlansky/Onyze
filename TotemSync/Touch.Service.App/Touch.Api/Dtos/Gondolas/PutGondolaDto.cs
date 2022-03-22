using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Grilla;

namespace Touch.Api.Dtos.Gondolas
{
    public class PutGondolaDto
    {
        [JsonProperty("titulo", Required = Required.Always)]
        [MinLength(4, ErrorMessage = "Por favor ingrese un titulo mas largo.")]
        public string Titulo { get; set; }

        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4, ErrorMessage = "Por favor ingrese un nombre mas largo.")]
        public string Nombre { get; set; }

        [JsonProperty("colorTitulo")]
        public string ColorTitulo { get; set; }

        [JsonProperty("colorFondo")]
        public string ColorFondo { get; set; }

        [JsonProperty("colorEncabezado")]
        public string ColorEncabezado { get; set; }

        [JsonProperty("idEncabezado")]
        public long IdEncabezado { get; set; }

        [JsonProperty("idFondo")]
        public long IdFondo { get; set; }

        [JsonProperty("imagen")]
        public string Imagen { get; set; }

        [JsonProperty("grilla")]
        public GrillaDeGondolaDto grilla { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }

        [JsonProperty("idCategoria")]
        public long IdCategoria { get; set; }
    }
}
