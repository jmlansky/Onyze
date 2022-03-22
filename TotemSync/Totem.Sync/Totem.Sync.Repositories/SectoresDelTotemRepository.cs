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
    public class SectoresDelTotemRepository: SingleEntityComunRepository<SectorDelTotem>, ISectoresDelTotemRepository
    {
        public SectoresDelTotemRepository(IConfiguration configuration): base (configuration)
        {

        }

        public async Task<List<SectorDelTotem>> GetPorIdTotem(long id)
        {            
            From += ", totem_sector ts";
            Where = $" WHERE ts.id_totem = @id " +
                $"AND {Alias}.id = ts.id_sector";
            Parameters = new Dictionary<string, object>() 
            {
                { "id", id}
            };
            Sql = Select + From + Where;
            return (List<SectorDelTotem>)await GetListOf<SectorDelTotem>(Sql, Parameters);
        }
    }
}
