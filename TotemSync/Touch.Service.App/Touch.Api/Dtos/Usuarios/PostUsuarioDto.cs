using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Usuarios
{
    public class PostUsuarioDto
    {
        [JsonProperty("nombre", Required = Required.Always)]
        [MinLength(4, ErrorMessage = "El nombre y apellido debe tener mas de 4 caracteres")]
        public string Nombre { get; set; }

        [JsonProperty("nombreUsuario", Required = Required.Always)]
        [MinLength(5, ErrorMessage = "El nombre de usuario debe tener mas de 5 caracteres")]
        public string NombreUsuario { get; set; }
        
        [JsonProperty("idCliente", Required = Required.Always)]
        [Range(1, double.MaxValue, ErrorMessage = "El id del cliente debe ser un valor numérico positivo")]
        public long IdCliente { get; set; }

        [JsonProperty("idRol", Required = Required.Always)]
        [Range(1, double.MaxValue, ErrorMessage = "El id del rol debe ser un valor numérico positivo")]
        public long IdRol { get; set; }

        [JsonProperty("mail", Required = Required.Always)]
        [MinLength(8, ErrorMessage = "El mail debe tener más de 8 caracteres")]
        public string Mail { get; set; }

        [JsonProperty("telefono")]
        [MaxLength(13, ErrorMessage = "El teléfono debe tener como máximo 13 caracteres")]
        public string Telefono { get; set; }
    }
}
