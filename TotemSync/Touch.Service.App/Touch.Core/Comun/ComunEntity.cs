using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun.Interfaces;

namespace Touch.Core.Comun
{
    public class ComunEntity: IComunEntity
    {
        [ColumnName("id")]
        public long Id { get; set; }

        [ColumnName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [ColumnName("creado")]
        public DateTime? Creado { get; set; }

        [ColumnName("modificado")]
        public DateTime? Modificado { get; set; }

        [ColumnName("eliminado")]
        public bool Eliminado { get; set; }
    }   
}
