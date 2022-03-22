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
    public class ProgramacionesRepository : SingleEntityComunRepository<Programacion>, IProgramacionesRepository
    {
        public ProgramacionesRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<List<Programacion>> GetPorIdPlaylist(long id)
        {
            Where = $" WHERE {Alias}.id_playlist = @id ";
            Sql = Select + From;
            return (List<Programacion>)await GetListOf<Programacion>(Sql, Parameters);
        }
    }
}
