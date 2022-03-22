using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Publicaciones
{
    public class PutPublicacionDto
    {
        [JsonProperty("nombre")]
        public string Nombre { get; set; }

        [JsonProperty("idArchivoFondo")]        
        public long IdArchivo { get; set; }


        [JsonProperty("idCliente")]
        public long IdCliente { get; set; }

        [JsonProperty("activo")]
        public bool Activo { get; set; }
    }
}
