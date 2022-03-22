using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Attributes;

namespace Touch.Core.Programaciones
{
    public interface IItem
    {
                 
        public long Id { get; set; }
        public DateTime? Creado { get; set; }
        public DateTime? Modificado { get; set; }
        public bool Eliminado { get; set; }

        [ColumnName("id_item")]
        public long IdItem { get; set; }

        [ColumnName("Id_programacion")]
        public long IdProgramacion { get; set; }

        [ColumnName("tipo")]
        public string Tipo { get; set; }
    }
}
