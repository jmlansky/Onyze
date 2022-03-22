using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Totems;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Totems.Contracts
{
    public interface ITotemsRepository : ISingleEntityComunRepository<Totem>
    {
        Task<IEnumerable<Totem>> GetFromSucursal(long idCliente, long? idSucursal, string[] columnsToignore = null);
        ICollection<Sector> GetSectoresFromTotem(long id);

        Task<IEnumerable<Totem>> Get(string nombre, long idCliente, long? idSucursal, string[] columnsToignore = null);

    }
}
