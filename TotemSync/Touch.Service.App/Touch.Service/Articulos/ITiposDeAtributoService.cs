using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface ITiposDeAtributoService
    {
        Task<IEnumerable<TipoAtributo>> Get();
        Task<ServiceResult> Insert(TipoAtributo tipoDeAtributo);
        Task<ServiceResult> Update(TipoAtributo tipoDeAtributo);
        Task<ServiceResult> Delete(TipoAtributo tipoDeAtributo);
        Task<TipoAtributo> Get(long id);
        Task<IEnumerable<TipoAtributo>> Get(string nombre);
    }
}
