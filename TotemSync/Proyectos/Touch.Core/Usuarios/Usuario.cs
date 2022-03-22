using Touch.Core.Attributes;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;

namespace Touch.Core.Usuarios
{
    [TableName("usuario")]
    public class Usuario : ComunEntity
    {
        [ColumnName("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [ColumnName("password")]
        public string Password { get; set; }

        public string PasswordViejo { get; set; }

        [ColumnName("mail")]
        public string Mail { get; set; }

        [ColumnName("telefono")]
        public string Telefono { get; set; }

        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }
        public Cliente Cliente { get; set; }

        public Sucursal Sucursal { get; set; }

        [ColumnName("id_rol")]
        public long IdRol { get; set; }
        public Auth.Rol Rol { get; set; }

        [ColumnName("requiere_cambiar_password")]
        public bool RequiereCambiarPassword { get; set; }
        
    }
}
