using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Publicaciones;
using Touch.Service.Comun;

namespace Touch.Service.Publicaciones
{
    public interface IPublicacionesService : ISingleEntityComunService<Publicacion>
    {
        Task<ServiceResult> DeleteObjectsFromPantalla(List<ObjetoAPublicar> objetos);
        Task<ServiceResult> InsertObjeto(ObjetoAPublicar item);      
    }
}
