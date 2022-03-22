using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;

namespace Touch.Repositories.Comun
{
    public interface ISingleEntityComunRepository<T> : IRepositoryBase<T>
    {
        Task<bool> Insert(T entity, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<bool> Update(T entity, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<bool> Delete(T entity, SqlTransaction tran, string[] columnsToIgnore = null);        
    }
}
