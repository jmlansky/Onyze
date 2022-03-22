using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Touch.Core.Archivos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Articulos
{
    public class Articulo : ComunEntity
    {
        [ColumnName("id_fabricante")]
        public long IdFabricante { get; set; }

        [ColumnName("id_tipo")]
        public long IdTipo { get; set; }

        [ColumnName("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [ColumnName("descripcion_larga")]
        public string DescripcionLarga { get; set; } = string.Empty;

        [ColumnName("etiquetas")]
        public string Etiquetas { get; set; } = string.Empty;

        [ColumnName("sku")]
        public string SKU { get; set; } = string.Empty;

        [ColumnName("precio")]
        public decimal Precio { get; set; } = 0;

        [ColumnName("prospecto")]
        public string Prospecto { get; set; } = string.Empty;                

        [ColumnName("activo")]
        public bool Activo { get; set; } = true;

        public bool Sponsoreado { get; set; } = false;

        public Fabricante Fabricante { get; set; } = new Fabricante();
        public TipoArticulo Tipo { get; set; } = new TipoArticulo();
        public List<Atributo> Atributos { get; set; } = new List<Atributo>();
        public List<CategoriaDeArticulo> Categorias { get; set; } = new List<CategoriaDeArticulo>();
        public List<CodigoDeBarras> Codigos { get; set; } = new List<CodigoDeBarras>();
        public List<Archivo> Archivos { get; set; } = new List<Archivo>();
    }
}
