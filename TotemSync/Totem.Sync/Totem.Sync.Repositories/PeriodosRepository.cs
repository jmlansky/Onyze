using Framework.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Totems.Sync.Repositories.Contracts;
using Touch.Core.Totems;

namespace Totems.Sync.Repositories
{
    public class PeriodosRepository : SingleEntityComunRepository<Periodo>, IPeriodosRepository
    {
        public PeriodosRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<IEnumerable<Periodo>> GetPorIdProgramacion(long id)
        {
            var where = Where + $" AND {Alias}.id_programacion = @id ";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            Sql = Select + From + where;
            return await GetListOf<Periodo>(Sql, Parameters);
        }
    }
}
