using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Usuarios;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Clientes.Contracts
{
    public interface IClientesRepository : ISingleEntityComunRepository<Cliente>
    {
        //Task<IEnumerable<Sucursal>> GetSucursales();

        //Task<IEnumerable<Usuario>> GetUsuarios(long id);

    }
}
