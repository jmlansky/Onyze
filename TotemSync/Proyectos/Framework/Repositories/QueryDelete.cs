using Framework.Attributes;
using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Repositories
{
    [QueryStatement("Delete")]
    public class QueryDelete<T> : IQueryStatement<T>
    {
        public Task<IEnumerable<T>> Excecute(string sql)
        {
            throw new NotImplementedException();
        }

        public Task<long> ExcecuteAndGetId(T entity, string[] columnsToIgnore = null)
        {
            throw new NotImplementedException();
        }
    }
}
