using System;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Promociones;

namespace Touch.Service.Promociones
{
    [TipoPromocion("ModificacionDePrecio")]
    public class ModificacionDePrecio : IPromociones
    {
        public void CalcularPromocion(Promocion promocion)
        {
        }

        public Task<bool> ValidarPromocion(Promocion promocion)
        {
            try
            {
                if (promocion.ItemsDePromocion == null || !promocion.ItemsDePromocion.Any())
                    return Task.FromResult(false);

                foreach (var detalle in promocion.ItemsDePromocion)
                {
                   if (((DetallePromocion)detalle).PrecioAnterior > ((DetallePromocion)detalle).PrecioActual &&
                            ((DetallePromocion)detalle).PrecioAnterior <= 0 )
                        return Task.FromResult(false);
                }

                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }
        }
    }
}
