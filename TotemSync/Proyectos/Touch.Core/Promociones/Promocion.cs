using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Articulos;
using Touch.Core.Attributes;
using Touch.Core.Comun;

namespace Touch.Core.Promociones
{
    [TableName("promocion")]
    public class Promocion: ComunEntity
    {
        [ColumnName("id_tipo_item")]
        public long IdTipoItem { get; set; }
       
        public string TipoItem { get; set; }

        public List<IItemDePromocion> ItemsDePromocion { get; set; }


        [ColumnName("id_tipo")]
        public int IdTipo { get; set; }
        public TipoPromocion Tipo { get; set; } = new TipoPromocion();

        [ColumnName("inicio")]
        public DateTime FechaInicio { get; set; }
        
        [ColumnName("fin")]
        public DateTime FechaFin { get; set; }

        [ColumnName("hasta_agotar_stock")]
        public bool HastaAgotarStock { get; set; }

        [ColumnName("porcentaje_descuento")]
        public decimal PorcentajeDescuento { get; set; } = 0;

        [ColumnName("cantidad_necesaria")]
        public long CantidadMinima { get; set; } = 0;

        [ColumnName("monto_descuento")]
        public decimal MontoFijo { get; set; } = 0;

        public List<PromocionDeProvincia> Provincias { get; set; } = new List<PromocionDeProvincia>();
        public List<PromocionDeRegiones> Regiones { get; set; } = new List<PromocionDeRegiones>();
        public List<PromocionDeCliente> Clientes { get; set; } = new List<PromocionDeCliente>();
        public List<PromocionDeGrupos> Grupos { get; set; } = new List<PromocionDeGrupos>();
    }
}
