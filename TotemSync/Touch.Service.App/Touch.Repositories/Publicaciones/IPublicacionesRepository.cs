using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Publicaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Publicaciones
{
    public interface IPublicacionesRepository : ISingleEntityComunRepository<Publicacion>
    {
        Task<bool> DeleteObjetosPublicados(long id, long idTipo);
        Task<bool> AsociarArchivoPublicacion(Publicacion publicacion);
        Publicacion EsArchivoDeAlgunaPublicacion(long id);
    }
}
