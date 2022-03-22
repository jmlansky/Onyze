using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("estante")]
    public class Estante : ComunEntity
    {
        [ColumnName("color")]
        public string Color { get; set; }

        [ColumnName("altura")]
        public decimal Altura { get; set; }

        [ColumnName("alto")]
        public decimal Alto { get; set; }

        [ColumnName("altura_contenedor_px")]
        public decimal AlturaContenedorPx { get; set; }

        [ColumnName("orden")]
        public long Orden { get; set; }

        [ColumnName("id_gondola")]
        public long IdGondola { get; set; }

        public List<ArticuloEstante> Articulos { get; set; } = new List<ArticuloEstante>();

        public List<EstanteDecoracion> Decoraciones { get; set; } = new List<EstanteDecoracion>();
    }
}
