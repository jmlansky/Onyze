using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Pubilicidades;

namespace Touch.Core.Publicaciones
{
    [TableName("objeto_publicitado")]
    public class ObjetoAPublicar: ComunEntity
    {
        [ColumnName("id_pantalla")]
        public long IdPantalla { get; set; }

        [ColumnName("id_tipo")]
        public long IdTipo { get; set; }

        [ColumnName("id_objeto")]
        public long IdObjeto { get; set; }

        public ComunEntity Objeto { get; set; }

        public TipoObjetoPublicitar Tipo { get; set; }
    }
}
