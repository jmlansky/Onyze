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
    public class MultimediaRepository: SingleEntityComunRepository<ItemProgramado>, IMultimediaRepository
    {
        public MultimediaRepository(IConfiguration configuration): base (configuration)
        {}

        public async Task<List<ItemProgramado>> GetPorIdPlaylist(long id)
        {  
            var where = Where + $" AND {Alias}.id_playlist = @id ";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            Sql = Select + From + where;
            return  (List<ItemProgramado>)await GetListOf<ItemProgramado>(Sql, Parameters);
        }
    }
}
