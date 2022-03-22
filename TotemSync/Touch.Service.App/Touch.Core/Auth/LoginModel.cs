using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Usuarios;

namespace Touch.Core.Auth
{
    public class LoginModel
    {
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public bool Autenticado { get; set; }
        public Usuario Usuario { get; set; }
    }
}
