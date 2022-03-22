using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Archivos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    [TableName("categoria")]
    public class CategoriaDeArticulo: ComunEntity
    {
        [ColumnName("id_categoria_padre")]
        public long? IdCategoriaPadre { get; set; }
                
        public string NombreCategoriaPadre { get; set; }

        public List<CategoriaDeArticulo> Subcategorias { get; set; } = new List<CategoriaDeArticulo>();

        [ColumnName("id_archivo")]
        public long? IdArchivo { get; set; }

        public Archivo Archivo { get; set; }

        [ColumnName("mostrar_totem")]
        public bool MostrarEnTotem { get; set; }
    }
}
