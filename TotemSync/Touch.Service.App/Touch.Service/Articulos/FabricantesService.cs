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
    public class FabricantesService : BaseService, IFabricantesService
    {
        private readonly IFabricantesRepository fabricantesRepository;
        public FabricantesService(IFabricantesRepository fabricantesRepository)
        {
            this.fabricantesRepository = fabricantesRepository;
        }

        public async Task<ServiceResult> Delete(int id)
        {
            return GetServiceResult(ServiceMethod.Insert, "Fanticante", await fabricantesRepository.Delete(new Fabricante() { Id = id }));
        }

        public async Task<IEnumerable<Fabricante>> Get()
        {
            return (IEnumerable<Fabricante>)await fabricantesRepository.Get();
        }

        public async Task<Fabricante> Get(int id)
        {
            return (Fabricante)await fabricantesRepository.Get(id);
        }

        public async Task<IEnumerable<object>> Get(string nombre)
        {
            return (IEnumerable<Fabricante>)await fabricantesRepository.Get(nombre);
        }

        public async Task<ServiceResult> Insert(Fabricante fabricante)
        {
            var existe = (await fabricantesRepository.Get(fabricante.Nombre)).Any(x => x.Nombre.ToLower().Equals(fabricante.Nombre.ToLower()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Fabricante");

            return GetServiceResult(ServiceMethod.Insert, "Fabricante", await fabricantesRepository.Insert(fabricante));
        }

        public async Task<ServiceResult> Update(Fabricante fabricante)
        {

            var fabricantes = await fabricantesRepository.Get(fabricante.Nombre);
            if (fabricantes.Any(x => x.Id != fabricante.Id && x.Nombre.ToUpper().Equals(fabricante.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Fabricante");

            var existe = await fabricantesRepository.Get(fabricante.Id);
            if (existe == null  || existe.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Fabricante");

            return GetServiceResult(ServiceMethod.Update, "Fabricante", await fabricantesRepository.Update(fabricante));
        }
    }
}
