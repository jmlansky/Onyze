using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IArticulosDestacadosRepository : ISingleEntityComunRepository<ArticuloDestacado>
    {
        Task<ArticuloDestacado> GetFromArticulo(long id, long idEstante);
        Task<bool> DeleteFromIdArticuloEstante(long id, SqlTransaction tran);
    }
}
