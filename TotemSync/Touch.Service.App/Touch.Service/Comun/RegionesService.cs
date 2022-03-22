using Microsoft.Extensions.Configuration;
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
    public class RegionesService : SingleEntityComunService<Region>, IRegionesService
    {
        private readonly ISingleEntityComunRepository<Region> regionesRepository;

        public RegionesService(ISingleEntityComunRepository<Region> regionesRepository) : base(regionesRepository)
        {
            this.regionesRepository = regionesRepository;
        }

        public override async Task<ServiceResult> InsertAndGetId(Region entity)
        {
            var regiones = await regionesRepository.Get(entity.Nombre);
            if (regiones != null && regiones.Any())
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Region");

            return await base.InsertAndGetId(entity);
        }

        public async override Task<ServiceResult> Update(Region entity)
        {
            var regiones = await regionesRepository.Get();

            if (!regiones.Any(x=> x.Id == entity.Id))
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Region");

            if (regiones.Any(x => x.Id != entity.Id && x.Nombre == entity.Nombre))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Region");

            entity.Modificado = DateTime.Now;
            return await base.Update(entity);
        }

    }
}
