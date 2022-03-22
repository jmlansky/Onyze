using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_periodos")]
    public class ProgramacionPeriodo : ComunEntity
    {
        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

        [ColumnName("fecha_inicio")]

        public DateTime FechaInicio { get; set; }

        [ColumnName("fecha_fin")]
        public DateTime FechaFin { get; set; }
        public List<ProgramacionFranjaHoraria> FranjasHorarias { get; set; } = new List<ProgramacionFranjaHoraria>();
        [ColumnName("dias")] 
        public string Dias { get; set; } 
    }
}
