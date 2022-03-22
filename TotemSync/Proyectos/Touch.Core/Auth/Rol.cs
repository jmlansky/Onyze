using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Auth
{
    [TableName("Rol")]
    public class Rol: ComunEntity
    {
        public List<PermisosDelRol> Permisos { get; set; } = new List<PermisosDelRol>();
    }
}
