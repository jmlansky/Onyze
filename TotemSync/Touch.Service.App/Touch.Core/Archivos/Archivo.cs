using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Archivos
{
    public class Archivo: ComunEntity
    {
        [ColumnName("url")]
        public string Url { get; set; }

        public TipoArchivo Tipo { get; set; } = new TipoArchivo();

        [ColumnName("id_tipo")]
        public long IdTipo { get; set; }

        [ColumnName("nombre_guardado")]
        public string NombreGuardado { get; set; }

        [ColumnName("id_articulo")]
        public long IdArticulo { get; set; }

        [ColumnName("id_archivo_original")]
        public long IdArchivoOriginal { get; set; }

        [ColumnName("size")]
        public string Size { get; set; }

        public IFormFile File { get; set; }

        public List<Archivo> Miniaturas { get; set; } = new List<Archivo>();

        [ColumnName("color_promedio")]
        public string ColorPromedio { get; set; }

        [ColumnName("width")]
        public int Width { get; set; }

        [ColumnName("height")]
        public int Height { get; set; }

    }
}
