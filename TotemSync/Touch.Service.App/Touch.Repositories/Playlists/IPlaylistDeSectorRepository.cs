using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{

    public interface IPlaylistDeSectorRepository : ISingleEntityComunRepository<PlaylistDeSector>
    {

        Task<bool> DeleteFromPlaylist(long id);
        Task<bool> DeleteFromPlaylist(long id, SqlTransaction transaction);
        Task<IEnumerable<PlaylistDeSector>> GetFromPlaylist(long id, string[] columnsToIgnore = null);
    }
}
