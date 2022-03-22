using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface ICategoriaAsociadaAlArticuloRepository: ISingleEntityComunRepository<CategoriaAsociadaAlArticulo>
    {
        IEnumerable<CategoriaDeArticulo> GetCategoriasDelArticulo(long articuloId);
        bool DeleteCategoriasDelArticulo(long id);
        Task DeleteCategoriasDelArticulo(long id, IEnumerable<long> categorias);
    }
}
