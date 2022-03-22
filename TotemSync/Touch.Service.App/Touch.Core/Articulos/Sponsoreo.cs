using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    [TableName("sponsoreo")]
    public class Sponsoreo : ComunEntity
    {
        [ColumnName("fecha_inicio")]
        public DateTime FechaInicio { get; set; }
        
        [ColumnName("fecha_fin")]
        public DateTime FechaFin { get; set; }

        [ColumnName("hora_inicio")]
        public TimeSpan HoraInicio { get; set; }

        [ColumnName("hora_fin")]
        public TimeSpan HoraFin { get; set; }

        [ColumnName("id_articulo")]
        public long IdArticulo { get; set; }

        [ColumnName("id_fabricante")]
        public long IdFabricante { get; set; }
        
    }
}
