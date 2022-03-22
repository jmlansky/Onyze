using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_playlists")]
    public class ProgramacionDePlaylist : ComunEntity 
    {
        [ColumnName("id_playlist")]
        public long IdPlaylist { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
