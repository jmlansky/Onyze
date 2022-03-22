using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Archivos.Contracts
{
    public interface IGetServices
    {
        Task<PagedResult> Get(int? pageNumber, int? pageSize);
        Task<IEnumerable<Archivo>> Get(string name);
        Task<Archivo> Get(long id);
        Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id);
        Task<PagedResult> GetFiltrados(FiltroArchivos filtros, int? pageNumber, int? pageSize);
        Task<IEnumerable<Archivo>> GetPorTipo(long idTipo);
        Task<IEnumerable<Archivo>> GetPorSize(string size);
        Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size);
        Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo);
        Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size);
    }
}
