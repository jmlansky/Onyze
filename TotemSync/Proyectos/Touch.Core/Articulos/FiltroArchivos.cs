using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Articulos
{
    public class FiltroArchivos
    {
        public string Nombre { get; set; }
        public string Size { get; set; }
        public long? IdTipoArchivo { get; set; }
        public DateTime? FechaAltaInicio { get; set; }
        public DateTime? FechaAltaFin { get; set; }
        public bool? Eliminado { get; set; }
    }
}
