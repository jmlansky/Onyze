using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("articulos_destacados")]
    public class ArticuloDestacado: ComunEntity
    {
        [ColumnName("id_articulo_estante")]
        public long IdArticuloEstante { get; set; }

        [ColumnName("posicion")]
        public string Posicion { get; set; }
    }
}
