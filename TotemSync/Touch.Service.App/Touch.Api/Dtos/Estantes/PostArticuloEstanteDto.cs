using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Touch.Api.Dtos.Estantes
{
    public class PostArticuloEstanteDto
    {
        public long IdArticulo { get; set; }
        public string Nombre { get; set; }
        public double OrigenX { get; set; }
        public double OrigenY { get; set; }
        public decimal CantidadX { get; set; }
        public decimal CantidadY { get; set; }
        public decimal Alto { get; set; }
        public decimal Ancho { get; set; }

        public int EstiloPrecio { get; set; }
        public string ColorEstiloPrecioFrente { get; set; }


        public string ColorEstiloPrecioFondo { get; set; }

      
        public bool MostrarPrecio { get; set; } 

        
        public bool MostrarNombre { get; set; }  

        public List<ArticuloDecoracionDto> Decoraciones { get; set; }
    }
}
