using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;

namespace Touch.Service.Sucursales
{
    public interface ISucursalesService
    {
        Task<PagedResult> Get(long idCliente, int? pageNumber, int? pageSize);
        Task<Sucursal> Get(long id, long idCliente);
        Task<PagedResult> Get(string nombre, long idCliente, int? pageNumber, int? pageSize);
        Task<ServiceResult> Insert(Sucursal sucursal);
        Task<ServiceResult> Update(Sucursal sucursal);
        Task<ServiceResult> Delete(long id);
    }
}
