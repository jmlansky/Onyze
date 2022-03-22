using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Comun.Interfaces
{
    public interface IComunEntity
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? Creado { get; set; }
        public DateTime? Modificado { get; set; }
        public bool Eliminado { get; set; }
    }
}
