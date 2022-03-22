using System;
using System.Collections.Generic;
using System.Text;

namespace Touch.Core.Promociones
{
    public class FiltrosDePromocion
    {
        public long Id { get; set; }
        public long IdTipo { get; set; }
        public List<PromocionDeProvincia> Provincias { get; set; }
        public List<PromocionDeRegiones> Regiones { get; set; }
        public List<PromocionDeCliente> Clientes { get; set; }
        public List<PromocionDeGrupos> Grupos { get; set; }
        public List<DetallePromocion> Articulos { get; set; } = new List<DetallePromocion>();
    }
}
