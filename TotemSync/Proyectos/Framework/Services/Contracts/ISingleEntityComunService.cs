using Framework.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Framework.Services.Contracts
{
    public interface ISingleEntityComunService<T>
    {
        Task<IEnumerable<T>> Get();
        Task<PagedResult> Get(int? pageNumber, int? pageSize);

        Task<IEnumerable<T>> Get(string name);
        Task<PagedResult> Get(string name, int? pageNumber, int? pageSize);

        Task<T> Get(long id);
        Task<IEnumerable<T>> GetPorIds(List<long> ids);

        Task<ServiceResult> Insert(T entity);
        Task<ServiceResult> InsertAndGetId(T entity);
        Task<ServiceResult> InsertAndGetId(T entity, string[] columnsToIgnore);
        Task<ServiceResult> Update(T entity);
        Task<ServiceResult> Update(T entity, string[] columnsToIgnore = null);
        Task<ServiceResult> Delete(T entity);
        
    }
}
