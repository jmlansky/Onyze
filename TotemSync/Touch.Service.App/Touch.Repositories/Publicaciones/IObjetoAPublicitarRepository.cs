using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Publicaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Publicaciones
{
    public interface IObjetoAPublicitarRepository : ISingleEntityComunRepository<ObjetoAPublicar>
    {
        //Task<bool> Insert(ObjetoAPublicar entity, SqlTransaction transaction, string[] columnsToIgnore = null);        
        Task<IEnumerable<ObjetoAPublicar>> GetFromPublicacion(long id, string[] columnsToIgnore = null);
        Task<IEnumerable<ObjetoAPublicar>> GetPorIdObjetoYTipo(long idObjeto, long idTipo, string[] columnsToIgnore = null);
        Task<bool> DeleteObjetosPublicados(ObjetoAPublicar entity);        
    }
}
