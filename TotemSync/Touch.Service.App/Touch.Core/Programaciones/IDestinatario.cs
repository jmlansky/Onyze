using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Attributes;

namespace Touch.Core.Programaciones
{
    public interface IDestinatario
    {
                 
        public long Id { get; set; }
        public DateTime? Creado { get; set; }
        public DateTime? Modificado { get; set; }
        public bool Eliminado { get; set; }

        [ColumnName("Id_programacion")]
        public long IdProgramacion { get; set; }

 
    }
}
