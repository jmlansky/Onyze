using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class SponsoreoRepository: SingleEntityComunRepository<Sponsoreo>, ISponsoreoRepository
    {
        public SponsoreoRepository(IConfiguration configuration): base(configuration)
        {
        }

        public async Task<IEnumerable<Sponsoreo>> Get(DateTime fechaVigente)
        {
            Sql = Select + From + Where + "and @fecha_vigente between fecha_inicio and fecha_fin";
            Parameters = new Dictionary<string, object>() { { "fecha_vigente", fechaVigente } };
            return await GetListOf<Sponsoreo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Sponsoreo>> Get(long idArticulo)
        {
            Sql = Select + From + Where + "and id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>() { { "id_articulo", idArticulo} };
            return await GetListOf<Sponsoreo>(Sql, Parameters);
        }
    }
}
