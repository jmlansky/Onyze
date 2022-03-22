using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Gondolas
{
    [TableName("grilla")]
    public class Grilla: ComunEntity
    {
        [ColumnName("grilla_x")]
        public int GrillaX { get; set; }

        [ColumnName("grilla_y")]
        public int GrillaY { get; set; }

        [ColumnName("es_plantilla")]
        public bool EsPlantilla { get; set; }

        [ColumnName("id_gondola")]
        public long IdGondola { get; set; }
    }
}
