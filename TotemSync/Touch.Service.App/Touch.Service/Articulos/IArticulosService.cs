using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface IArticulosService
    {
        Task<IEnumerable<Articulo>> GetArticulos(bool incluirDetalles = true);
        Task<ArticulosPaginados> GetArticulosPaginadosFiltrados(int pageNumber, int pageSize, string nombre);

        Task<ArticulosPaginados> GetArticulosPaginados(int pageNumber, int pageSize, bool incluirDetalles = true);

        Task<Articulo> GetArticulo(long id, bool incluirDetalles = true);
        Task<List<Articulo>> GetArticulosAlternativos(long id, bool incluirDetalles = true);
        Task<IEnumerable<Articulo>> GetArticulosCruzados(long id, bool incluirDetalles = true);
        Task<List<Articulo>> GetArticulos(Articulo articulo);
        Task<ServiceResult> InsertarCruzados(IEnumerable<ArticuloMultiple> articulos);        
        Task<ServiceResult> InsertarAlternativos(IEnumerable<ArticuloMultiple> articulos);
        Task<ServiceResult> DeleteCruzado(ArticuloMultiple articulo);
        Task<ServiceResult> DeleteAlternativo(ArticuloMultiple articulo);      
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> AsociarAtributosAlArticulo(long id, IEnumerable<long> idsAtributos);      
        Task<IEnumerable<Atributo>> GetAtributosAlArticulo(long id);
        Task<ServiceResult> DeleteAtributosDelArticulo(long id, IEnumerable<long> idsAtributos);
        Task<ServiceResult> Insert(Articulo articulo);
        Task<ServiceResult> Update(Articulo articulo);
        Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id);
        Task<PagedResult> GetWithDeleted(int? pageNumber, int? pageSize, string fechaSincro);
        Task<PagedResult> GetRelaciones(int? pageNumber, int? pageSize, string fechaSincro);
        Task<long> GetCurentCount();
    }
}
