using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface IAtributosRepository : IRepository
    {
        Task<IEnumerable<Atributo>> GetAtributosDelArticulo(long articuloId);
        Task<bool> InsertarAtributosDelArticulo(long articuloId, IEnumerable<Atributo> atributos);
        Task<bool> InsertarAtributoDelArticulo(long id, long idAtributo);
        Task<bool> UpdateAtributoDelArticulo(long id, long idAtributo);
        Task<bool> DeleteAtributosDelArticulo(long id, long idAtributo);
        Task<bool> DeleteAtributosDelArticulo(long id);
        Task<IEnumerable<Atributo>> GetAtributosDelTipo(long idTipo);
    }
}
