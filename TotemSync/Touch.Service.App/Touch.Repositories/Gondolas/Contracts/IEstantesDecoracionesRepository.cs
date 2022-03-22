using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IEstantesDecoracionesRepository : ISingleEntityComunRepository<EstanteDecoracion>
    {
        Task<bool> DeleteFromEstante(long id, SqlTransaction tran);
        Task<List<EstanteDecoracion>> GetDecoracionesDeEstante(long id);
        Task<bool> Insert(List<EstanteDecoracion> decoraciones, long id, DateTime date, SqlTransaction tran);
    }
}
