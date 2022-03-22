using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Pubilicidades;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Publicaciones
{
    public interface ITipoObjetoPublicitarRepository: ISingleEntityComunRepository<TipoObjetoPublicitar>
    {
        Task<IEnumerable<TipoObjetoPublicitar>> GetPorTipo(string tag, string[] columnsToIgnore = null);
    }
}
