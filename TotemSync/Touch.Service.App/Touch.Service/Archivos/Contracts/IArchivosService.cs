using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Service.Comun;

namespace Touch.Service.Archivos.Contracts
{
    public interface IArchivosService: ISingleEntityComunService<Archivo>
    {
        Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id);
        Task<ServiceResult> UploadFile(Archivo archivo, bool small, bool medium, bool large);
        Task<ServiceResult> UploadFileAsociarAlArticulo(long idArticulo, Archivo archivo, bool small, bool medium, bool large);
        Task<ServiceResult> UploadFileAsociarAGondola(long idGondola, string referencia, Archivo archivo, bool small);
        Task<ServiceResult> UploadFileAsociarACategoria(long idCategoria, Archivo archivo, bool small, bool medium, bool large);
        Task<ServiceResult> UploadFileAsociarAPublicacion(long idPublicacion, Archivo archivo, bool small, bool medium, bool large);
        Task<ServiceResult> UpdateFile(Archivo archivo);
        Task<IEnumerable<Archivo>> GetPorTipo(long idTipo);
        Task<IEnumerable<Archivo>> GetPorSize(string size);
        Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size);
        Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo);
        Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size);
        Task<PagedResult> GetFiltrados(FiltroArchivos filtros, int? pageNumber, int? pageSize);
    }
}
