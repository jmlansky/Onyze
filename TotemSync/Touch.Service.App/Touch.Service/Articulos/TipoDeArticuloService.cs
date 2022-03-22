using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Articulos
{
    public class TipoDeArticuloService: BaseService, ITipoDeArticuloService
    {
        private readonly ITipoDeArticuloRepository tipoDeArticuloRepository;
        public TipoDeArticuloService(ITipoDeArticuloRepository tipoDeArticuloRepository)
        {
            this.tipoDeArticuloRepository = tipoDeArticuloRepository;
        }

        public async Task<ServiceResult> Delete(int id)
        {
            return GetServiceResult(ServiceMethod.Delete, "Tipo de Artículo", await tipoDeArticuloRepository.Delete(new TipoArticulo() { Id = id }));
        }

        public async Task<IEnumerable<TipoArticulo>> Get()
        {
            return (IEnumerable<TipoArticulo>)await tipoDeArticuloRepository.Get();
        }

        public async Task<TipoArticulo> Get(int id)
        {
            return (TipoArticulo)await tipoDeArticuloRepository.Get(id);
        }

        public async Task<IEnumerable<TipoArticulo>> Get(string nombre)
        {
            return (IEnumerable<TipoArticulo>)await tipoDeArticuloRepository.Get(nombre);
        }

        public async Task<ServiceResult> Insert(TipoArticulo tipo)
        {
            var tipos = await tipoDeArticuloRepository.Get(tipo.Nombre);
            if (tipos.Any(x => x.Nombre.ToUpper().Equals(tipo.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de Artículo");
            return GetServiceResult(ServiceMethod.Insert, "Tipo de Artículo", await tipoDeArticuloRepository.Insert(tipo));
        }
        public async Task<ServiceResult> Update(TipoArticulo tipo)
        {
            var tipos = await tipoDeArticuloRepository.Get(tipo.Nombre);
            if (tipos.Any(x => x.Id != tipo.Id && x.Nombre.ToUpper().Equals(tipo.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Tipo de Artículo");

            var tipoArticulo = await tipoDeArticuloRepository.Get(tipo.Id);
            if (tipoArticulo != null && tipoArticulo.Id > 0)
                return GetServiceResult(ServiceMethod.Update, "Tipo de Artículo", await tipoDeArticuloRepository.Update(tipo));

            return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Tipo de Artículo");
        }
    }
}
