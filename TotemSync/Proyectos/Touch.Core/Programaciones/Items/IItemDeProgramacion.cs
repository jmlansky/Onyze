using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Programaciones.Items
{
    public interface IItemDeProgramacion
    {
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}
