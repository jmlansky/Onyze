using Framework.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Totem.Sync.Repositories.Contracts;
using CoreTotem = Touch.Core.Totems;

namespace Totem.Sync.Repositories
{
    public class TotemsRepository: SingleEntityComunRepository<CoreTotem.Totem>, ITotemsRepository
    {
        public TotemsRepository(IConfiguration configuration) : base(configuration)
        {}

        public async Task<List<CoreTotem.Totem>> Syncronizar(long idTotem, DateTime fecha)
        {          
            Select = "SELECT " + GetColumnsForSelect(Alias);
            Sql = Select + From + Where;
            return (List<CoreTotem.Totem>)await GetListOf<CoreTotem.Totem>(Sql);            
        }
    }
}
