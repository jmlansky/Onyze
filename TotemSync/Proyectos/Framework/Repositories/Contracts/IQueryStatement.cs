using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Repositories.Contracts
{
    public interface IQueryStatement<T>
    {
        Task<long> ExcecuteAndGetId(T entity, string[] columnsToIgnore = null);
        Task<IEnumerable<T>> Excecute(string sql);
    }
}
