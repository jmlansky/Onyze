using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;

namespace Touch.Service.Promociones
{
    public interface IItemsDePromocionService
    {
        Task<IEnumerable<IItemDePromocion>> GetItemDePromociones(long idPromocion);
    }
}
