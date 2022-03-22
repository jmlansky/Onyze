using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Totems;

namespace Totem.Sync.Repositories.Contracts
{
    public interface IPlaylistsRepository : ISingleEntityComunRepository<Playlist>
    {
        Task<List<Playlist>> GetPorIdSector(long id);
        Task<List<Playlist>> GetPlaylistsSinSector();
    }
}
