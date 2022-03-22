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
    public class ItemsProgramacionRepository: SingleEntityComunRepository<ProgramacionItem>, IItemsProgramacionRepository
    {
        public ItemsProgramacionRepository(IConfiguration configuration): base (configuration)
        {
        }

        public async Task<List<ProgramacionItem>> GetPorTipo(string tipo)
        {
            Where = $" WHERE {Alias}.tipo = @tipo and {Alias}.eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "tipo", tipo}
            };
            Sql = Select + From + Where;
            return (List<ProgramacionItem>)await GetListOf<ProgramacionItem>(Sql, Parameters);
        }
    }
}
