using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Programaciones
{
    public interface IProgramacionPeriodoRepository : ISingleEntityComunRepository<ProgramacionPeriodo>
    {
        public Task<bool> Insertar(ProgramacionPeriodo programacionPeriodo, SqlTransaction tran);
        Task<IEnumerable<ProgramacionPeriodo>> GetFromProgramacion(long idProgramacion, string[] columnsToIgnore = null);
        Task<bool> DeleteFromProgramacion(long idProgramacion, SqlTransaction tran, string[] columnsToIgnore = null);
    }
}