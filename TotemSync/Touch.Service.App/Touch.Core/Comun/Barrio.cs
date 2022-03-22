using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;

namespace Touch.Core.Comun
{
    public class Barrio: ComunEntity
    {
        [ColumnName("id_localidad")]
        public long IdLocalidad { get; set; }

        [ColumnName("codigo_postal")]
        public string CodigoPostal { get; set; }
    }
}
