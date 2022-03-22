using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;

namespace Touch.Service.Promociones
{
    public class Oferta : IPromociones
    {
        public void CalcularPromocion(Promocion promocion)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidarPromocion(Promocion promocion)
        {
            throw new NotImplementedException();
        }
    }
}
