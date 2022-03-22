using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_franjas_horarias")]
    public class ProgramacionFranjaHoraria : ComunEntity
    {
        [ColumnName("hora_desde")]
        public string HoraDesde { get; set; }

        [ColumnName("hora_hasta")]
        public string HoraHasta { get; set; }

        [ColumnName("id_programaciones_periodo")]
        public long IdProgramacionesPeriodo { get; set; }
    }
}
