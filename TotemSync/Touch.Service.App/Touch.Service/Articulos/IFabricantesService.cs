using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface IFabricantesService
    {
        Task<IEnumerable<Fabricante>> Get();
        Task<Fabricante> Get(int id);
        Task<ServiceResult> Update(Fabricante fabricante);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> Insert(Fabricante fabricante);
        Task<IEnumerable<object>> Get(string nombre);
    }
}
