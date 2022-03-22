using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    public class ArticuloMultiple: ComunEntity
    {
        public long IdOrigen { get; set; }
        public long IdDestino { get; set; }
    }
}
