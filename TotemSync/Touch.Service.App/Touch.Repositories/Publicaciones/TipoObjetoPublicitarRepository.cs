using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Pubilicidades;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Publicaciones
{
    public class TipoObjetoPublicitarRepository : SingleEntityComunRepository<TipoObjetoPublicitar>, ITipoObjetoPublicitarRepository
    {
        public TipoObjetoPublicitarRepository(IConfiguration configuration): base (configuration)
        {
        }

        public async Task<IEnumerable<TipoObjetoPublicitar>> GetPorTipo(string tag, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and upper(" + Alias + ".tags) like upper(@tags)";
            Parameters = new Dictionary<string, object>() { { "tags", "%" + tag + "%" } };
            return await GetListOf<TipoObjetoPublicitar>(Sql, Parameters);
        }
    }
}
