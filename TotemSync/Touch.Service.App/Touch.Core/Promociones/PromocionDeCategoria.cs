using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("promociones_categorias")]
    public class PromocionDeCategoria: ComunEntity, IItemDePromocion
    {
        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }

        [ColumnName("id_categoria")]
        public long IdCategoria { get; set; }
        public decimal PrecioAnterior { get ; set; }
        public decimal PrecioActual { get; set ; }
    }
}
