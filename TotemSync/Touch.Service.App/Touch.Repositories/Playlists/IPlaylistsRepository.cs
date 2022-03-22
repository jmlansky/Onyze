using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public interface IPlaylistsRepository : ISingleEntityComunRepository<Playlist>
    {
        //Task<bool> DeleteMultimedia(long id, long idTipo);
        //Task<bool> AsociarArchivoPublicacion(Playlist playlist);
    }
}
