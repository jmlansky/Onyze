using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Playlists
{
    [TableName("playlists_sectores")]
    public class PlaylistDeSector : ComunEntity
    {
        [ColumnName("id_playlist")]
        public long IdPlaylist { get; set; }

        [ColumnName("id_sector")]
        public long IdSector { get; set; }
 
    }
}
