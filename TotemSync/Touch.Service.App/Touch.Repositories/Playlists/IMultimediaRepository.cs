using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public interface IMultimediaRepository : ISingleEntityComunRepository<Multimedia>
    {
        //Task<bool> Insert(ObjetoAPublicar entity, SqlTransaction transaction, string[] columnsToIgnore = null);        
        Task<IEnumerable<Multimedia>> GetFromPlaylist(long id, string[] columnsToIgnore = null);
        Task<IEnumerable<Multimedia>> GetPorIdObjetoYTipo(long idObjeto, long idTipo, string[] columnsToIgnore = null);
        Task<bool> DeleteFromPlaylist(long id);
        Task<bool> DeleteFromPlaylist(long id, SqlTransaction transaction);

    }
}
