using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Promociones;
using Touch.Service.Comun;

namespace Touch.Service.Promociones
{
    public interface IPromocionesService : ISingleEntityComunService<Promocion>
    {
        Task<IEnumerable<Promocion>> GetPorFiltro(FiltrosDePromocion filtros, int pageNumber, int pageSize);
    }
}
