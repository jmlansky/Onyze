using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Touch.Repositories.Promociones
{
    public interface IDestinatarioDePromocionRepository<T>
    {
        Task<IEnumerable<T>> GetFromPromocion(long idPromocion, string[] columnsToIgnore = null);
        Task<bool> DeleteFromPromocion(long idPromocion, SqlTransaction tran, string[] columnsToIgnore = null);
    }
}
