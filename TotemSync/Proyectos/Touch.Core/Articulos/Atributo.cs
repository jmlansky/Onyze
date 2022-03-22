using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    public class Atributo: ComunEntity
    {
        public long IdTipo { get; set; }
        public TipoAtributo TipoAtributo { get; set; } = new TipoAtributo();
    }
}
