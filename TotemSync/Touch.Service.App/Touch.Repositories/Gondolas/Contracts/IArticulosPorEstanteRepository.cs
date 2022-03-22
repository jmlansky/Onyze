using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IArticulosPorEstanteRepository: ISingleEntityComunRepository<ArticuloEstante>
    {
        Task<IEnumerable<ArticuloEstante>> GetArticulosPorEstante(long id, SqlTransaction transaction=null);
        Task<bool> DeleteAllArticulosDelEstante(long idEstante);
        Task<bool> DeleteAllArticulosDelEstante(long idEstante, SqlTransaction tran);
        Task<bool> Insert(List<ArticuloEstante> articulos, long idEstante, DateTime dateTime, SqlTransaction tran);
    }
}
