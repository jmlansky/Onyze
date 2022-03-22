using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface ICategoriasDeArticulosService
    {
        Task<CategoriaDeArticulo> Get(long id);
        Task<IEnumerable<CategoriaDeArticulo>> Get(string nombre);
        Task<IEnumerable<CategoriaDeArticulo>> Get();
        Task<ServiceResult> Insert(CategoriaDeArticulo categoria);
        Task<ServiceResult> Update(CategoriaDeArticulo categoria);
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> AsociarArchivoCategoria(CategoriaDeArticulo categoria);
    }
}
