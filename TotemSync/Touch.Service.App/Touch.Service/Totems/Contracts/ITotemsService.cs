using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Totems;

namespace Touch.Service.Totems
{
    public interface ITotemsService
    {
        Task<PagedResult> GetFromSucursal(long idCliente,long? idSucursal, int? pageNumber, int? pageSize);
        Task<Totem> Get(long id);
        Task<PagedResult> Get(string nombre, long idCliente, long? idSucursal, int? pageNumber, int? pageSize);
        Task<ServiceResult> Insert(Totem  totem);
        Task<ServiceResult> Update(Totem  totem);
        Task<ServiceResult> Delete(long id);
    }
}
