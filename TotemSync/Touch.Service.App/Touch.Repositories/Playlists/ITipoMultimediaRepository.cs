using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public interface ITipoMultimediaRepository : ISingleEntityComunRepository<TipoMultimedia>
    {
        Task<IEnumerable<TipoMultimedia>> GetPorTipo(string tag, string[] columnsToIgnore = null);
    }
}
