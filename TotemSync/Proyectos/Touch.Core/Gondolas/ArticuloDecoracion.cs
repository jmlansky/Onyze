using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Archivos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("articulos_decoraciones")]
    public class ArticuloDecoracion: ComunEntity
    {
        [ColumnName("id_articulo_estante")]
        public long IdArticuloEstante { get; set; }

        [ColumnName("id_archivo")]
        public long IdArchivo { get; set; }

        public Archivo Archivo { get; set; }

        [ColumnName("muestra_precio")]
        public bool MuestraPrecio { get; set; }

        [ColumnName("posicion")]
        public long Posicion { get; set; }

        [ColumnName("posicion_inicial")]
        public string PosicionInicial { get; set; }


        [ColumnName("ancho")]
        public long Ancho { get; set; }

        [ColumnName("alto")]
        public long Alto { get; set; }

        [ColumnName("desplazamiento_x")]
        public long DesplazamientoX { get; set; }

        [ColumnName("desplazamiento_y")]
        public long DesplazamientoY { get; set; }


        public ArticuloDestacado Destacado { get; set; }
    }
}
