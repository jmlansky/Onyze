using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Publicaciones;

namespace Touch.Core.Playlists
{
    public class GetPlaylist : ComunEntity
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("tiempo_predeterminado")]
        public int TiempoPredeterminado { get; set; }


        public List<Multimedia> Multimedia { get; set; } = new List<Multimedia>();

        public List<Sector> Sectores { get; set; } = new List<Sector>();

        [ColumnName("activo")]
        public bool Activo { get; set; }


    }
}
