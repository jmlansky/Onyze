using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Touch.Core.Programaciones;

namespace Touch.Repositories.Programaciones
{
    public interface IProgramacionItemRepository
    {
        public Task<bool> Insert(IItem item, SqlTransaction tran, string[] columnsToIgnore = null);
        Task<IEnumerable<IItem>> GetFromProgramacion(long idProgramacion, string[] columnsToIgnore = null);

        Task<bool> DeleteFromProgramacion(long idProgramacion, SqlTransaction tran, string[] columnsToIgnore = null);
    }
}