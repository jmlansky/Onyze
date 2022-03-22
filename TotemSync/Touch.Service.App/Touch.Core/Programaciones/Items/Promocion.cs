using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Programaciones.Items
{
    public class Promocion : IItemDeProgramacion
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}
