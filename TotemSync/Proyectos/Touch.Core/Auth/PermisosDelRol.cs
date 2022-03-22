using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Auth
{
    [TableName("permisos_roles")]
    public class PermisosDelRol: ComunEntity
    {
        [ColumnName("id_permiso")]
        public long IdPermiso { get; set; }
        public string Permiso { get; set; }

        [ColumnName("id_rol")]
        public long IdRol { get; set; }
        public string Rol { get; set; }
    }
}
