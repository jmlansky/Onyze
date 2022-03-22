using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Framework.Repositories.Contracts
{
    public interface ISingleEntityComunRepository<T> : IRepositoryBase<T>
    {
        Task<bool> Insert(T entity, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<bool> Update(T entity, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<bool> Delete(T entity, SqlTransaction tran, string[] columnsToIgnore = null);        
    }
}
