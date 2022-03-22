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
    public class ZonasService : BaseService, IZonasService
    {
        private readonly IZonasRepository zonasRepository;
        public ZonasService(IZonasRepository zonasRepository)
        {
            this.zonasRepository = zonasRepository;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            return  GetServiceResult(ServiceMethod.Delete, "Zonas", await zonasRepository.Delete(new Zona() { Id = id }));
        }

        public async Task<IEnumerable<Zona>> Get()
        {
            return (IEnumerable<Zona>)await zonasRepository.Get();
        }

        public async Task<Zona> Get(long id)
        {
            return (Zona)await zonasRepository.Get(id);
        }

        public async Task<IEnumerable<Zona>> Get(string nombre)
        {
            return (IEnumerable<Zona>)await zonasRepository.Get(nombre);
        }

        public async Task<ServiceResult> Insert(Zona zona)
        {
            var existe = (await zonasRepository.Get(zona.Nombre)).Any(x => x.Nombre.ToUpper().Equals(zona.Nombre.ToUpper()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Zonas");

            return GetServiceResult(ServiceMethod.Insert, "Zonas", await zonasRepository.Insert(zona));
        }

        public async Task<ServiceResult> Update(Zona zona)
        {

            var zonas = await zonasRepository.Get();

            if (!zonas.Any(x=> x.Id == zona.Id))
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Zonas");

            if (zonas.Any(x => x.Id != zona.Id && x.Nombre == zona.Nombre))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Zonas");

            return GetServiceResult(ServiceMethod.Update, "Zonas", await zonasRepository.Update(zona));
        }
    }
}
