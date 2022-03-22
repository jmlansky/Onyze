using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;
using Touch.Core.Pubilicidades;

namespace Touch.Core.Publicaciones
{
    [TableName("pantalla")]
    public class Publicacion:ComunEntity
    {
        [ColumnName("id_archivo_fondo")]
        public long IdArchivo { get; set; }

        [ColumnName("activo")]
        public bool Activo { get; set; }

        public Archivo Archivo { get; set; }        

        public List<ObjetoAPublicar> ObjetosAPublicitar { get; set; } = new List<ObjetoAPublicar>();
    }
}
