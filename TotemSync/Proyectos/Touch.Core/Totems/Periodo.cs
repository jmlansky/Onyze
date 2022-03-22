using Framework.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Comun;

namespace Touch.Core.Totems
{
    [TableName("programaciones_periodos")]
    public class Periodo : ComunEntity
    {
        [ColumnName("dias")]
        public string Dias { get; set; }

        [ColumnName("fecha_inicio")]
        public DateTime FechaInicio { get; set; }

        [ColumnName("fecha_fin")]
        public DateTime FechaFin { get; set; }

        public List<FranjaHoraria> FranjasHorarias { get; set; } = new List<FranjaHoraria>();
    }
}
