using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Clientes;
using Touch.Core.Comun;

namespace Touch.Service.Clientes
{
    public interface IClientesService
    {
        Task<PagedResult> Get(int? pageNumber, int? pageSize);
        Task<Cliente> Get(long id);
        Task<PagedResult> Get(string nombre, int? pageNumber, int? pageSize);
        Task<ServiceResult> Insert(Cliente cliente);
        Task<ServiceResult> Update(Cliente cliente);
        Task<ServiceResult> Delete(long id);
    }
}
