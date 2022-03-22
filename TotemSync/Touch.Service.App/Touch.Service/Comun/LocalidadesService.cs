using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Repositories.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Comun
{
    public class LocalidadesService : BaseService, ILocalidadesService
    {
        private readonly ILocalidadesRepository localidadRepository;
        private readonly IProvinciasService provinciasService;
        public LocalidadesService(ILocalidadesRepository localidadRepository, IProvinciasService provinciasService)
        {
            this.localidadRepository = localidadRepository;
            this.provinciasService = provinciasService;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            return GetServiceResult(ServiceMethod.Delete, "Localidad", await localidadRepository.Delete(new Localidad() { Id = id }));
        }

        public async Task<IEnumerable<Localidad>> Get()
        {
            return (IEnumerable<Localidad>)await localidadRepository.Get();
        }

        public async Task<IEnumerable<Localidad>> Get(string nombre)
        {
            return  (IEnumerable<Localidad>)await localidadRepository.Get(nombre);
        }

        public async Task<Localidad> Get(long id)
        {
            return  (Localidad)await localidadRepository.Get(id);
        }

        public async Task<ServiceResult> Insert(Localidad localidad)
        {

            var prov = await provinciasService.Get(localidad.IdProvincia);
            if (prov == null || prov.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Provincia");

            var existe = (await localidadRepository.Get(localidad.Nombre)).Any(x => x.Nombre.ToUpper().Equals(localidad.Nombre.ToUpper()) && ((Localidad)x).IdProvincia == localidad.IdProvincia);
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Localidades");
            
            return GetServiceResult(ServiceMethod.Insert, "Localidad", await localidadRepository.Insert(localidad));
        }

        public async Task<ServiceResult> Update(Localidad localidad)
        {
            var loc = await localidadRepository.Get(localidad.Id);
            if (loc == null || loc.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Localidades");

            var prov = await provinciasService.Get(localidad.IdProvincia);
            if (prov == null || prov.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Provincia");

            var existe = (await localidadRepository.Get(localidad.Nombre)).Any(x => x.Id != localidad.Id && x.Nombre.ToUpper().Equals(localidad.Nombre.ToUpper()) && ((Localidad)x).IdProvincia == localidad.IdProvincia );
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Localidades");

            return GetServiceResult(ServiceMethod.Update, "Localidad", await localidadRepository.Update(localidad));
        }
    }
}
