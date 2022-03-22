using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Playlists;
using Touch.Service.Comun;

namespace Touch.Service.Playlists
{
    public interface IPlaylistsService : ISingleEntityComunService<Playlist>
    {
        Task<ServiceResult> DeleteObjectsFromPantalla(List<Multimedia> objetos);
        Task<ServiceResult> InsertObjeto(Multimedia item);      
    }
}
