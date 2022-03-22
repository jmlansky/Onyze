using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    [TableName("categorias_articulos")]
    public class CategoriaAsociadaAlArticulo: ComunEntity
    {
        [ColumnName("id_articulo")]
        public long IdArticulo{ get; set; }

        [ColumnName("id_categoria")]
        public long IdCategoria { get; set; }
    }
}
