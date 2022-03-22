using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_sponsoreos")]
    public class ProgramacionDeSponsoreo : ComunEntity 
    {
        [ColumnName("id_sponsoreo")]
        public long IdSponsoreo { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

    }


}
