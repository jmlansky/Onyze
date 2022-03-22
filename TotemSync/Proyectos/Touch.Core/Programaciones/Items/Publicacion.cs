using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Programaciones.Items;

namespace Touch.Core.Programaciones
{
    public class Publicacion : IItemDeProgramacion
    {        
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }        
    }
}
