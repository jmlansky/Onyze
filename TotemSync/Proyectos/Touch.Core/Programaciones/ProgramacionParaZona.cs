using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_zonas")]
    public class ProgramacionParaZona : ComunEntity 
    {
        [ColumnName("id_zona")]
        public long IdZona { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
