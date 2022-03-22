using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;
using Touch.Service.Comun;

namespace Touch.Service.Articulos
{
    public interface ICodigosDeBarraService : ISingleEntityComunService<CodigoDeBarras>
    {
        Task<IEnumerable<CodigoDeBarras>> GetCodigosDelArticulo(long id);
    }
}
