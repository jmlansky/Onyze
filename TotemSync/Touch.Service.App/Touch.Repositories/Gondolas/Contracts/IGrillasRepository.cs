using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IGrillasRepository : ISingleEntityComunRepository<Grilla>
    {
        Task DeleteFromGondola(long id, SqlTransaction tran);
        Grilla GetFromGondola(long id);
    }
}
