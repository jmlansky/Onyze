using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Archivos
{
    public class PostBuscarArchivosFiltradosDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("idTipoArchivo")]
        public long? IdTipoArchivo { get; set; }

        [JsonProperty("fechaAltaInicio")]
        public DateTime? FechaAltaInicio { get; set; }

        [JsonProperty("fechaAltaFin")]
        public DateTime? FechaAltaFin { get; set; }

        [JsonProperty("eliminado")]
        public bool? Eliminado { get; set; }
    }
}
