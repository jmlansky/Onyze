using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Comun.Interfaces;

namespace Touch.Core.Promociones
{
    public interface IItemDePromocion
    {
        public long IdPromocion { get; set; }
        public string Nombre { get; set; }
        public DateTime? Creado { get; set; }
        public DateTime? Modificado { get; set; }
        public bool Eliminado { get; set; }
        public decimal PrecioAnterior { get; set; }
        public decimal PrecioActual { get; set; }
        public long Id { get; set; }
    }
}
