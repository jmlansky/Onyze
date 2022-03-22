using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Playlists
{
    [TableName("tipo_multimedia")]
    public class TipoMultimedia : ComunEntity
    {
   
        [ColumnName("tags")]
        public string Tags { get; set; }
    }
}
