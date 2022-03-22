using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface IGetArticulosRepository 
    {
        Task<IEnumerable<Articulo>> GetArticulosPorFabricante(string nombre);
        Task<IEnumerable<Articulo>> GetArticulosPorIdFabricante(long id);
        Task<IEnumerable<Articulo>> GetArticulosPorCategoria(string nombre);
        Task<IEnumerable<Articulo>> GetArticulosPorIdCategoria(long id);
        Task<IEnumerable<Articulo>> GetArticulosPorTipo(string nombre);
        Task<IEnumerable<Articulo>> GetArticulosPorIdTipo(long id);
        Task<IEnumerable<Articulo>> GetArticulosPorAtributos(IEnumerable<string> nombres);
        Task<IEnumerable<Articulo>> GetArticulosPorAtributos(string nombre);
        Task<IEnumerable<Articulo>> GetArticulosPorCodigo(string ean);
        Task<IEnumerable<Articulo>> GetArticulosPorEstadoActivo(bool activo);
        Task<IEnumerable<Articulo>> GetPorIds(List<long> idArticulos);
        Task<IEnumerable<Articulo>> GetPorSKU(string sku);
    }
}
