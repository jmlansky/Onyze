using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Usuarios;

namespace Touch.Api.Dtos.Auth
{
    public class LoginResponse
    {
        public string Messsage { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }

        public GetUsuarioDto Usuario { get; set; }
    }
}
