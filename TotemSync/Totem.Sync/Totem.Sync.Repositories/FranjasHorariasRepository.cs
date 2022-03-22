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
    public class FranjasHorariasRepository: SingleEntityComunRepository<FranjaHoraria>, IFranjasHorariasRepository
    {
        public FranjasHorariasRepository(IConfiguration configuration): base(configuration)
        {

        }

        public async Task<IEnumerable<FranjaHoraria>> GetPorPeriodo(long id)
        {
            var where = Where + $" AND {Alias}.id_programaciones_periodo = @id ";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            Sql = Select + From + where;
            return await GetListOf<FranjaHoraria>(Sql, Parameters);
        }
    }
}
