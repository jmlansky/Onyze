using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Repositories.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Comun
{
    public class BarriosService : BaseService, IBarriosService
    {
        private readonly ISingleEntityComunRepository<Barrio> barriosRepository;
        private readonly ISingleEntityComunRepository<Localidad> localidadesRepository;

        public BarriosService(ISingleEntityComunRepository<Barrio> barriosRepository, ISingleEntityComunRepository<Localidad> localidadesRepository)
        {
            this.barriosRepository = barriosRepository;
            this.localidadesRepository = localidadesRepository;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            var result = await barriosRepository.Delete(new Barrio() { Id = id, Modificado = DateTime.Now });
            return GetServiceResult(ServiceMethod.Delete, "Barrios", result);
        }

        public async Task<IEnumerable<Barrio>> Get()
        {
            return await barriosRepository.Get();
        }

        public async Task<Barrio> Get(long id)
        {
            return await barriosRepository.Get(id);
        }

        public async Task<IEnumerable<Barrio>> Get(string nombre)
        {
            return await barriosRepository.Get(nombre);
        }

        public async Task<ServiceResult> Insert(Barrio barrio)
        {

            var existeLocalidad = await localidadesRepository.Get(barrio.IdLocalidad);
            if (existeLocalidad == null || existeLocalidad.Id == 0)
                return new ServiceResult() { Message = "No existe esta localidad", HasErrors = true, Method = ServiceMethod.Insert.ToString(), StatusCode = ServiceMethodsStatusCode.Error };

            var existe = (await barriosRepository.Get(barrio.Nombre)).Any(x => x.Nombre.ToUpper().Equals(barrio.Nombre.ToUpper()) && ((Barrio)x).IdLocalidad == barrio.IdLocalidad);
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Barrio");

            barrio.Creado = DateTime.Now;
            return GetServiceResult(ServiceMethod.Insert, "Barrio", await barriosRepository.Insert(barrio));
        }

        public async Task<ServiceResult> Update(Barrio barrio)
        {

            var existente = await barriosRepository.Get(barrio.Id);
            if (existente == null || existente.Id == 0)
                return new ServiceResult() { Message = "No existe el barrio en esta localidad", HasErrors = true, Method = ServiceMethod.Insert.ToString(), StatusCode = ServiceMethodsStatusCode.Error };

            var existeLocalidad = await localidadesRepository.Get(barrio.IdLocalidad);
            if (existeLocalidad == null || existeLocalidad.Id == 0)
                return new ServiceResult() { Message = "No existe la esta localidad", HasErrors = true, Method = ServiceMethod.Insert.ToString(), StatusCode = ServiceMethodsStatusCode.Error };

            var existe = (await barriosRepository.Get(barrio.Nombre)).Any(x => x.Id != barrio.Id && x.Nombre.ToUpper().Equals(barrio.Nombre.ToUpper()) && ((Barrio)x).IdLocalidad == barrio.IdLocalidad );
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Barrio");

            barrio.Modificado = DateTime.Now;
            return GetServiceResult(ServiceMethod.Update, "Barrio", await barriosRepository.Update(barrio));
        }
    }
}
