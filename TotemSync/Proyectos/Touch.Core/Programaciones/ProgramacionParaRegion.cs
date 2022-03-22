using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_regiones")]
    public class ProgramacionParaRegion : ComunEntity 
    {
        [ColumnName("id_region")]
        public long IdRegion { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
