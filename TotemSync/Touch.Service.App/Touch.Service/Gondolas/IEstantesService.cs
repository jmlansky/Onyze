using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Service.Comun;

namespace Touch.Service.Gondolas
{
    public interface IEstantesService : ISingleEntityComunService<Estante>
    {
        Task<IEnumerable<Estante>> GetEstantesDeLaGondola(long id);

        Task<ServiceResult> AsociarArticulosAlEstante(IEnumerable<ArticuloEstante> entities);

        Task<ServiceResult> EliminarArticulosDelEstante(IEnumerable<ArticuloEstante> entities);

        Task<ServiceResult> EliminarArticulosDelEstante(long id);

        Task<ServiceResult> ActualizarArticulosDelEstante(IEnumerable<ArticuloEstante> entities);

        Task<IEnumerable<ArticuloEstante>> GetArticulosDelEstante(long id);
    }
}
