using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("grupos_categorias")]
    public class DetalleDeGrupos : ComunEntity
    {
        [ColumnName("id_grupo")]
        public long IdGrupo { get; set; }

        [ColumnName("id_promocion")]
        public long IdPromocion { get; set; }
    }
}
