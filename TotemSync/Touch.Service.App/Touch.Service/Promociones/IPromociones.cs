using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;

namespace Touch.Service.Promociones
{
    public interface IPromociones
    {
        void CalcularPromocion(Promocion promocion);
        Task<bool> ValidarPromocion(Promocion promocion);
    }
}
