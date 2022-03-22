using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Pubilicidades
{
    [TableName("tipo_objeto_publicitar")]
    public class TipoObjetoPublicitar:ComunEntity
    {
        [ColumnName("tags")]
        public string Tags { get; set; }
    }
}
