using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("articulos_estantes")]
    public class ArticuloEstante : ComunEntity
    {
        [ColumnName("id_estante")]
        public long IdEstante { get; set; }

        [ColumnName("id_articulo")]
        public long IdArticulo { get; set; }

        [ColumnName("origen_x")]
        public double OrigenX { get; set; }

        [ColumnName("origen_y")]
        public double OrigenY { get; set; }

        [ColumnName("cantidad_x")]
        public decimal CantidadX { get; set; }

        [ColumnName("cantidad_y")]
        public decimal CantidadY { get; set; }

        [ColumnName("alto")]
        public decimal Alto { get; set; }

        [ColumnName("ancho")]
        public decimal Ancho { get; set; }

        [ColumnName("mostrar_precio")]
        public bool MostrarPrecio { get; set; }  

        [ColumnName("mostrar_nombre")]
        public bool MostrarNombre { get; set; }  

        [ColumnName("estilo_precio")]
        public int EstiloPrecio { get; set; }

        [ColumnName("color_estilo_precio_frente")]
        public string ColorEstiloPrecioFrente { get; set; }

        [ColumnName("color_estilo_precio_fondo")]
        public string ColorEstiloPrecioFondo { get; set; }


        public List<ArticuloDecoracion> Decoraciones { get; set; } = new List<ArticuloDecoracion>();
        public bool EsDestacado() { return false; }//Decoracion.Destacado != null; }

        public List<CodigoDeBarras> CodigosDeBarra { get; set; } = new List<CodigoDeBarras>();
    }
}
