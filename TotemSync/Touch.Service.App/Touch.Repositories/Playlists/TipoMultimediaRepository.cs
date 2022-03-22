using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public class TipoMultimediaRepository : SingleEntityComunRepository<TipoMultimedia>, ITipoMultimediaRepository
    {
        public TipoMultimediaRepository(IConfiguration configuration): base (configuration)
        {
        }

        public async Task<IEnumerable<TipoMultimedia>> GetPorTipo(string tag, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and upper(" + Alias + ".tags) like upper(@tags)";
            Parameters = new Dictionary<string, object>() { { "tags", "%" + tag + "%" } };
            return await GetListOf<TipoMultimedia>(Sql, Parameters);
        }
    }
}
