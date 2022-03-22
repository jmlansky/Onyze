using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("articulos_promociones")]
    public class DetallePromocion: ComunEntity, IItemDePromocion
    {
        [ColumnName("id_articulo")]
        public long IdArticulo { get; set; }

        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }       

        [ColumnName("precio_anterior")]
        public decimal PrecioAnterior { get; set; }

        [ColumnName("precio_actual")]
        public decimal PrecioActual { get; set; }        

        public Articulo Articulo { get; set; }
    }
}
