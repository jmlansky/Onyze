using System.Threading.Tasks;
using Touch.Core.Promociones;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Promociones
{
    [TipoPromocion("DescuentoPorcentual")]
    public class DescuentoPorcentual : IPromociones
    {
        public void CalcularPromocion(Promocion promocion)
        {
            if (promocion.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                foreach (var articulo in promocion.ItemsDePromocion)
                {
                    if (articulo.GetType() == typeof(DetallePromocion))
                    {
                        var precio = ((DetallePromocion)articulo).PrecioAnterior - (((DetallePromocion)articulo).PrecioAnterior * (promocion.PorcentajeDescuento / 100));
                        ((DetallePromocion)articulo).PrecioActual = precio;
                    }
                }
            }                        
        }

        public Task<bool> ValidarPromocion(Promocion promocion)
        {
            return Task.FromResult(promocion.PorcentajeDescuento > 0);
        }
    }
}
