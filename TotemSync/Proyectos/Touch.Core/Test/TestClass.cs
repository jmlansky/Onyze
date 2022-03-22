using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Test
{
    [TableName("Test")]
    public class TestClass: ComunEntity
    {
        /// <summary>
        /// ComunEntity tiene definidas las propiedades:
        /// Id, Nombre, Creado, Modificado, Eliminado
        /// </summary>
    }
}
