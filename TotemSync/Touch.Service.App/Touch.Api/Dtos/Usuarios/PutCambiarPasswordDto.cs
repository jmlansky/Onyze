using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Usuarios
{
    public class PutCambiarPasswordDto
    {
        [JsonProperty("nombreUsuario", Required = Required.Always)]
        [MinLength(8, ErrorMessage = "El nombre de usuario debe tener mas de 8 caracteres")]
        public string NombreUsuario { get; set; }

        [JsonProperty("passwordViejo", Required = Required.Always)]
        [MinLength(64, ErrorMessage = "El password debe tener mas de 64 caracteres")]
        public string PasswordViejo { get; set; }

        [JsonProperty("passwordNuevo", Required = Required.Always)]
        [MinLength(64, ErrorMessage = "El password debe tener mas de 64 caracteres")]
        public string Password { get; set; }

        [JsonProperty("confirmacionPassword", Required = Required.Always)]
        [MinLength(64, ErrorMessage = "El password debe tener mas de 64 caracteres")]
        public string ConfirmacionPassword { get; set; }
    }
}
