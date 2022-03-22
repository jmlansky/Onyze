using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface ICategoriasDeArticuloRepository: ISingleEntityComunRepository<CategoriaDeArticulo>
    {
        Task<IEnumerable<CategoriaDeArticulo>> GetCategoriasDelArticulo(long articuloId);
        Task<bool> AsociarArchivoCategoria(CategoriaDeArticulo categoria);
        CategoriaDeArticulo EsArchivoDeAlgunaCategoria(long id);
    }
}
