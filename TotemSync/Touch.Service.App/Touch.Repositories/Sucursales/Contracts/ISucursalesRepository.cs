using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Sucursales;
using Touch.Core.Usuarios;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Sucursales.Contracts
{
    public interface ISucursalesRepository : ISingleEntityComunRepository<Sucursal>
    {
        Task<IEnumerable<Sucursal>> Get(string nombre, long idCliente, string[] columnsToIgnore = null);

        Task<IEnumerable<Sucursal>> Get(long idCliente, string[] columnsToIgnore = null);

        Task<Sucursal> Get(long id, long idCliente, string[] columnsToIgnore=null);


    }
}
