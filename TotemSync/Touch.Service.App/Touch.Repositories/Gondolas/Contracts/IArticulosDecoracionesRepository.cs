using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IArticulosDecoracionesRepository : ISingleEntityComunRepository<ArticuloDecoracion>
    {
        Task<List<ArticuloDecoracion>> GetFromArticulo(long idArticulo, long idEstante);
        Task<bool> DeleteFromIdArticuloEstante(long id, SqlTransaction tran);
    }
}
