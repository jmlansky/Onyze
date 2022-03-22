using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    [TableName("codigo_barra")]
    public class CodigoDeBarras: ComunEntity
    {
        [ColumnName("id_articulo")]
        public long IdArticulo { get; set; }

        [ColumnName("ean")]
        public string EAN { get; set; }
    }
}
