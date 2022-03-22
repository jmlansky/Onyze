using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Archivos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("estante_decoracion")]
    public class EstanteDecoracion : ComunEntity
    {
        [ColumnName("id_estante")]
        public long IdEstante { get; set; }

        [ColumnName("texto")]
        public string Texto { get; set; } = string.Empty;

        [ColumnName("posicion")]
        public long Posicion { get; set; }

        [ColumnName("color")]
        public string Color { get; set; }

        [ColumnName("ancho")]
        public long Ancho { get; set; }

        [ColumnName("alto")]
        public long Alto { get; set; }

        [ColumnName("desplazamiento_x")]
        public long DesplazamientoX { get; set; }

        [ColumnName("desplazamiento_y")]
        public long DesplazamientoY { get; set; }

        [ColumnName("posicion_inicial")]
        public long PosicionInicial { get; set; }

        [ColumnName("id_archivo")]
        public long? IdArchivo { get; set; }

        public Archivo Archivo { get; set; }
    }
}
