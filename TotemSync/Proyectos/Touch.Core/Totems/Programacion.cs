using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Totems.Contracts;

namespace Touch.Core.Totems
{
    [TableName("programacion")]
    public class Programacion : ComunEntity
    {
        [ColumnName("activa")]
        public bool Activa { get; set; }

        public List<ItemProgramado> ItemsProgramados { get; set; } = new List<ItemProgramado>();

        public List<Periodo> Periodos { get; set; } = new List<Periodo>();
    }
}
