using Framework.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Totem.Sync.Repositories.Contracts;
using Touch.Core.Totems;

namespace Totem.Sync.Repositories
{
    public class PlaylistsRepository: SingleEntityComunRepository<Playlist>, IPlaylistsRepository
    {
        public PlaylistsRepository(IConfiguration configuration): base (configuration)
        {

        }

        public async Task<List<Playlist>> GetPlaylistsSinSector()
        {
            var from = From + $" JOIN playlists_sectores ps ON {Alias}.id = ps.id_playlist";
            var where = Where + $" AND ps.eliminado = 0 and ps.id_sector is null";           
            Sql = Select + from + where;
            return (List<Playlist>)await GetListOf<Playlist>(Sql);
        }

        public async Task<List<Playlist>> GetPorIdSector(long id)
        {            
            var from = From + ", playlists_sectores ps";
            var where = Where + 
                $" AND ps.id_sector = @id " +
                $"AND {Alias}.id = ps.id_playlist " +
                $"AND ps.eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            Sql = Select + from + where;
            return (List<Playlist>)await GetListOf<Playlist>(Sql, Parameters);
        }
    }
}
