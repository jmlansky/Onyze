using System.Collections.Generic;

namespace Touch.Core.Usuarios
{
    public class RolDelUsuario
    {
        public long IdRol { get; set; }
        public string Nombre { get; set; }
        public List<PermisosDelRolDelUsuario> Permisos { get; set; }
    }
}
