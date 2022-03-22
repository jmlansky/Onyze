using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_provincias")]
    public class ProgramacionParaProvincia : ComunEntity 
    {
        [ColumnName("id_provincia")]
        public long IdProvincia { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
