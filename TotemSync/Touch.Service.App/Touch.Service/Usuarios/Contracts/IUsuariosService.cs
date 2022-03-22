using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Usuarios;
using Touch.Service.Comun;

namespace Touch.Service.Usuarios.Contracts
{
    public interface IUsuariosService : ISingleEntityComunService<Usuario>
    {
        Task<ServiceResult> UpdatePassword(Usuario usuario);
        Task<PagedResult> GetFromCliente(long idCliente, string name, int? pageNumber, int? pageSize);
    }
}
