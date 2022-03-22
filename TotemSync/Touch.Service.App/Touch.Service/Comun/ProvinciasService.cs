using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Repositories.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Comun
{
    public class ProvinciasService: BaseService, IProvinciasService
    {
        private readonly IProvinciasRepository provinciasRepository;
        private readonly ILocalidadesRepository localidadesRepository;
        public ProvinciasService(IProvinciasRepository provinciasRepository, ILocalidadesRepository localidadesRepository)
        {
            this.provinciasRepository = provinciasRepository;
            this.localidadesRepository = localidadesRepository;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            return GetServiceResult(ServiceMethod.Delete, "Localidad", await provinciasRepository.Delete(new Provincia() { Id = id}));
        }

        public async Task<IEnumerable<Provincia>> Get()
        {
            return (IEnumerable<Provincia>)await provinciasRepository.Get();
        }

        public async Task<Provincia> Get(long id)
        {
            return (Provincia)await provinciasRepository.Get(id);
        }

        public async Task<IEnumerable<Provincia>> Get(string nombre)
        {
            return (IEnumerable<Provincia>)await provinciasRepository.Get(nombre);
        }

        public async Task<IEnumerable<Localidad>> GetLocalidades(long id)
        {
            return (IEnumerable<Localidad>)await localidadesRepository.GetAll(id);
        }

     

        public async Task<ServiceResult> Insert(Provincia provincia)
        {
            var existe = (await provinciasRepository.Get(provincia.Nombre)).Any(x=> x.Nombre.ToUpper().Equals(provincia.Nombre.ToUpper()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Provincia");

            return GetServiceResult(ServiceMethod.Insert, "Provincia", await provinciasRepository.Insert(provincia));
        }

        public async Task<ServiceResult> Update(Provincia provincia)
        {
            var provincias = await provinciasRepository.Get(provincia.Nombre);
            if (provincias.Any(x => x.Id != provincia.Id && x.Nombre.ToUpper().Equals(provincia.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Provincia");

            var prov = await provinciasRepository.Get(provincia.Id);
            if (prov == null || prov.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Provincia");

            return GetServiceResult(ServiceMethod.Update, "Provincia", await provinciasRepository.Update(provincia));
        }
    }
}
