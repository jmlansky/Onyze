using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Invariants;
using Touch.Core.Promociones;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Promociones
{
    [TipoPromocion("DescuentoFijo")]
    public class MontoFijo : IPromociones
    {
        public void CalcularPromocion(Promocion promocion)
        {
            if (promocion.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                foreach (var articulo in promocion.ItemsDePromocion)
                    ((DetallePromocion)articulo).PrecioActual = ((DetallePromocion)articulo).PrecioAnterior - promocion.MontoFijo;
            }            
        }

        public Task<bool> ValidarPromocion(Promocion promocion)
        {
            var resultadoDeValidadorDePrecios = true;

            // valida que no haya detalles con precios negativos o cero
            if (promocion.TipoItem == TiposDeItemsDePromocion.Articulos.ToString())
            {
                resultadoDeValidadorDePrecios = !promocion.ItemsDePromocion.Any(x => ((DetallePromocion)x).PrecioAnterior <= 0);
                return Task.FromResult(promocion.MontoFijo > 0 && resultadoDeValidadorDePrecios);
            }

            return Task.FromResult(promocion.MontoFijo > 0 && resultadoDeValidadorDePrecios);

        }
    }
}
