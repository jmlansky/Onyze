using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("promociones_fabricantes")]
    public class PromocionDeFabricantes: ComunEntity, IItemDePromocion
    {
        [ColumnName("id_fabricante")]
        public long IdFabricante { get; set; }

        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }

        public decimal PrecioAnterior { get ; set; }
        public decimal PrecioActual { get; set ; }
    }
}
