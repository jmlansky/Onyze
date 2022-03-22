using System;
using System.Collections.Generic;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Programaciones;
using Touch.Core.Programaciones.Items;

namespace Touch.Core.Programaciones
{
    [TableName("programacion")]
    public class Programacion: ComunEntity
    {
      
        public List<IItem> Items { get; set; } = new List<IItem>();
        public List<ProgramacionPeriodo> Periodos { get; set; } = new List<ProgramacionPeriodo>();

        [ColumnName("activa")]
        public bool Activa { get; set; }

    }
}
