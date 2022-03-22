using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Auth
{
    public class LoginDto
    {
        [Required]
        public string NombreUsuario { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
