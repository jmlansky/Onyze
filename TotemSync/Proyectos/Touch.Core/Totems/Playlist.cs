using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Totems.Contracts;

namespace Touch.Core.Totems
{
    [TableName("playlist")]
    public class Playlist: ComunEntity, IProgramacionItem
    {
        [ColumnName("id_cliente")]
        public long IdCliente { get; set; }

        [ColumnName("tiempo_predeterminado")]
        public int TiempoPredeterminado { get; set; }

        [ColumnName("activo")]
        public bool Activo { get; set; }

        public List<ItemProgramado> ItemsProgramados { get; set; } = new List<ItemProgramado>();
    }
}
