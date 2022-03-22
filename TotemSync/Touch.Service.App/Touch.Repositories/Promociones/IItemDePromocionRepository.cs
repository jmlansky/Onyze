using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Promociones
{
    public interface IItemDePromocionRepository<T>: ISingleEntityComunRepository<T>
    {
        Task<IEnumerable<T>> GetPorPromocion(long idPromocion, string[] columnsToIgnore = null);
        Task<bool> DeleteFromPromocion(long idPromocion, SqlTransaction tran, string[] columnsToIgnore = null);
    }
}
