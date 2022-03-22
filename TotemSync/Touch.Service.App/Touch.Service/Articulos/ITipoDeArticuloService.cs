using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface ITipoDeArticuloService
    {
        Task<IEnumerable<TipoArticulo>> Get();
        Task<ServiceResult> Delete(int id);
        Task<TipoArticulo> Get(int id);
        Task<IEnumerable<TipoArticulo>> Get(string nombre);
        Task<ServiceResult> Insert(TipoArticulo tipo);
        Task<ServiceResult> Update(TipoArticulo tipo);
    }
}
