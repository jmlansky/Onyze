using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Articulos
{
    public interface IAtributosService
    {
        Task<IEnumerable<Atributo>> GetAtributosDelArticulo(long articuloId, bool incluyeTipo = true);
        Task<IEnumerable<Atributo>> GetAtributosDelTipo(long id);

        Task<ServiceResult> InsertarAtributosDelArticulo(long articuloId, List<Atributo> atributos);
        Task<IEnumerable<Atributo>> Get();
        Task<Atributo> Get(long id);
        Task<IEnumerable<Atributo>> Get(string nombre);
        Task<ServiceResult> Insert(Atributo atributo);
        Task<ServiceResult> Delete(long id);
        Task<ServiceResult> Update(Atributo atributo);
        Task<ServiceResult> InsertarAtributosDelArticulo(long id, IEnumerable<long> idsAtributos);        
        Task<ServiceResult> DeleteAtributosDelArticulo(long id, IEnumerable<long> idsAtributos);
        Task<ServiceResult> DeleteAtributosDelArticulo(long id);
    }
}
