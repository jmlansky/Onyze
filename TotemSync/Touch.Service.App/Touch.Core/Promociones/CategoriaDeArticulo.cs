using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("categoria")]
    public class CategoriaDeArticulo: ComunEntity
    {
        [ColumnName("id_categoria_padre")]
        public long? IdCategoriaPadre { get; set; }

        public List<CategoriaDeArticulo> Subcategorias { get; set; } = new List<CategoriaDeArticulo>();

    }
}
