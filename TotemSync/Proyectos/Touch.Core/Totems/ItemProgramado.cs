using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Comun;
using static Touch.Core.Totems.Enumerations.Enums;

namespace Touch.Core.Totems
{
    [TableName("playlists_multimedia")]
    public class ItemProgramado: ComunEntity
    {
        [ColumnName("id_tipo")]
        public long IdTipo { get; set; }

        [ColumnName("id_playlist")]
        public long IdPlaylist { get; set; }

        [ColumnName("id_objeto")]
        public long IdObjeto { get; set; }

        [ColumnName("tiempo")]
        public int Tiempo { get; set; }

        public TipoObjetoPlaylistEnum Tipo { get; set; }
    }
}
