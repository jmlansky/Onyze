using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;

namespace Touch.Core.Totems
{
    [TableName("totem")]
    public class Totem : ComunEntity
    {

        [ColumnName("serial")]
        public string Serial { get; set; }

        [ColumnName("id_sucursal")]
        public long IdSucursal { get; set; }

        public Sucursal Sucursal { get; set; }

        public List<long> IdSectores { get; set; } = new List<long>();

        public List<SectorDelTotem> Sectores { get; set; } = new List<SectorDelTotem>();
    }
}
