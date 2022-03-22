using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Pubilicidades;

namespace Touch.Core.Playlists
{
    [TableName("playlists_multimedia")]
    public class Multimedia : ComunEntity
    {
        [ColumnName("id_playlist")]
        public long IdPlaylist { get; set; }

        [ColumnName("id_tipo")]
        public long IdTipo { get; set; }

        [ColumnName("id_objeto")]
        public long IdObjeto { get; set; }

        public ComunEntity Objeto { get; set; }

        public TipoMultimedia Tipo { get; set; }

        [ColumnName("tiempo")]
        public int Tiempo { get; set; }

        public string Url { get; set; }

    }
}
