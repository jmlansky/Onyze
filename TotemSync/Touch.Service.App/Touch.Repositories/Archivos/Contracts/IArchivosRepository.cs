using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Archivos.Contracts
{
    public interface IArchivosRepository : ISingleEntityComunRepository<Archivo>
    {
        Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id);
        Task<IEnumerable<Archivo>> GetPorTipo(long idTipo);
        Task<IEnumerable<Archivo>> GetPorSize(string size);
        Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size);
        Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size);
        Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo);
        Task<bool> DeleteAllFromOriginal(long id);
        Task<bool> DeleteAllButOriginal(long id);
        Task<IEnumerable<Archivo>> GetFiltrados(FiltroArchivos filtros, string[] columnsToIgnore);
       
    }
}
