using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Programaciones
{
    [TableName("programaciones_items")]
    public class ProgramacionItem : ComunEntity , IItem
    {
        [ColumnName("id_item")]
        public long IdItem { get; set; }

        [ColumnName("id_programacion")]
        public long IdProgramacion { get; set; }

        [ColumnName("tipo")]
        public string Tipo { get; set; }
    }


}
