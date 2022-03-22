using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("promociones_clientes")]
    public class PromocionDeCliente: ComunEntity
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }
    }
}
