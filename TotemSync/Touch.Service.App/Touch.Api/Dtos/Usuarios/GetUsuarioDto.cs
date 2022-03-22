using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Cliente;

namespace Touch.Api.Dtos.Usuarios
{
    public class GetUsuarioDto: ComunDto
    {
        [JsonProperty("nombreUsuario")]
        public string NombreUsuario { get; set; }

        [JsonProperty("cliente")]
        public string Cliente { get; set; }

        [JsonProperty("rol")]
        public string rol { get; set; }

        [JsonProperty("mail")]
        public string mail { get; set; }


        [JsonProperty("telefono")]
        public string telefono { get; set; }

    }
}
