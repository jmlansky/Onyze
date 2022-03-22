using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;

namespace Touch.Core.Comun
{
    public class Localidad: ComunEntity
    {
        [ColumnName("id_provincia")]
        public long IdProvincia { get; set; }
    }
}
