using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_grupos")]
    public class ProgramacionParaGrupo : ComunEntity 
    {
        [ColumnName("id_grupo")]
        public long IdGrupo { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
