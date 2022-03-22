using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.ResultadosPaginados;

namespace Touch.Core.Articulos
{
    public class ArticulosPaginados : ResultadoPaginado
    {
        public List<Articulo> Articulos { get; set; } = new List<Articulo>();

    }
}
