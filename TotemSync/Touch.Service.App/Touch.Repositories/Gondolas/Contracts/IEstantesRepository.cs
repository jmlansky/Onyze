using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IEstantesRepository : ISingleEntityComunRepository<Estante>
    {
        Task<IEnumerable<Estante>> GetEstantesDeLaGondola(long id, SqlTransaction transaction=null);
        Task<bool> DeleteFromGondola(long id);
        Task<bool> DeleteFromGondola(long id, SqlTransaction transaction);
    }
}
