using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Touch.Repositories.Comun
{
    public interface IRepositoryBase<T>
    {
        Task<T> Get(long id, string[] columnsToIgnore = null);
        Task<IEnumerable<T>> Get(List<long> ids, string[] columnsToIgnore = null);

        Task<IEnumerable<T>> Get(string[] columnsToIgnore = null);
        Task<IEnumerable<T>> Get(string nombre, string[] columnsToIgnore = null);

        Task<bool> Insert(T entity, string[] columnsToIgnore = null);
        Task<long> InsertAndGetId(T entity, string[] columnsToIgnore = null);
        Task<long> InsertAndGetId(T entity, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<bool> Update(T entity, string[] columnsToIgnore = null);
        Task<bool> Delete(T entity, string[] columnsToIgnore = null);
    }
}
