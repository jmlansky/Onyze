using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("promociones_regiones")]
    public class PromocionDeRegiones: ComunEntity
    {
        [ColumnName("id_region")]
        public long IdRegion { get; set; }

        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }
    }
}
