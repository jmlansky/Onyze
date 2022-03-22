using System.Data.SqlClient;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface ITiposDeAtributoRepository: IRepository
    {
        Task<long> Insert(ComunEntity entity, SqlConnection connection);
    }
}
