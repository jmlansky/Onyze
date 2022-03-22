using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Publicaciones;

namespace Touch.Core.Playlists
{
    public class Playlist : ComunEntity
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("tiempo_predeterminado")]
        public int TiempoPredeterminado { get; set; }


        public List<Multimedia> Multimedia { get; set; } = new List<Multimedia>();

        public List<PlaylistDeSector> PlaylistDeSector { get; set; } = new List<PlaylistDeSector>();

        public List<Sector> Sector { get; set; } = new List<Sector>();


        [ColumnName("activo")]
        public bool Activo { get; set; }


    }
}
