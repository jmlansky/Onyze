using Framework.Repositories;
using Framework.Repositories.Contracts;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Totem.Sync.Repositories.Contracts;
using Touch.Core.Totems;

namespace Totem.Sync.Repositories
{
    public class TipoMultimediaRepository: SingleEntityComunRepository<TipoMultimedia>, ITipoMultimediaRepository
    {
        public TipoMultimediaRepository(IConfiguration configuration): base(configuration)
        {

        }

        public async Task<TipoMultimedia> GetPorTipo(long id)
        {
            Where = $" WHERE {Alias}.id_tipo = @id ";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            Sql = Select + From + Where;
            return await Get<TipoMultimedia>(Sql, Parameters);
        }
    }
}
