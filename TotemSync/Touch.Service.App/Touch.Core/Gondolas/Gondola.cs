using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("gondola")]
    public class Gondola: ComunEntity
    {
        public Gondola()
        {
            ColorTitulo = string.Empty;
            ColorFondo = string.Empty;
            ColorEncabezado = string.Empty;
        }

        [ColumnName("titulo")]
        public string Titulo { get; set; }

        [ColumnName("color_titulo")]
        public string ColorTitulo { get; set; } = string.Empty;

        [ColumnName("color_fondo")]
        public string ColorFondo { get; set; } = string.Empty;

        [ColumnName("color_encabezado")]
        public string ColorEncabezado { get; set; } = string.Empty;

        [ColumnName("id_encabezado")]
        public long IdEncabezado { get; set; }

        [ColumnName("id_fondo")]
        public long IdFondo { get; set; }

        [ColumnName("imagen")]
        public DateTime Imagen { get; set; }

        
        public string Image { get; set; }

        public List<Estante> Estantes { get; set; } = new List<Estante>();
        public Grilla Grilla { get; set; } = new Grilla();

        [ColumnName("activo")]
        public bool Activo { get; set; }

        [ColumnName("id_categoria")]
        public long? IdCategoria { get; set; }

        public CategoriaDeArticulo Categoria { get; set; }
    }
}
