using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Sucursales;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Sucursales.Contracts;
using Touch.Service.Comun;
using Touch.Service.Sucursales;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Sucursales
{
    public class SucursalesService : SingleEntityComunService<Sucursal>, ISucursalesService
    {
        private readonly ISucursalesRepository sucursalesRepository;
        private readonly IBarriosRepository barriosRepository;
        private readonly IProvinciasRepository provinciasRepository;
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly IZonasRepository zonasRepository;
        private readonly IRegionesRepository regionesRepository;

        private string[] columnsToIgnore = { "Barrio", "Localidad", "Provincia", "Zona", "Cliente","Region" };

        public IProvinciasRepository ProvinciasRepository => provinciasRepository;

        public SucursalesService(ISucursalesRepository sucursalesRepository,
            IBarriosRepository barriosRepository,
            IProvinciasRepository provinciasRepository,
             ILocalidadesRepository localidadesRepository,
             IZonasRepository zonasRepository,
             IRegionesRepository regionesRepository

                ) : base(sucursalesRepository)
        {
            this.sucursalesRepository = sucursalesRepository;
            this.zonasRepository = zonasRepository;
            this.provinciasRepository = provinciasRepository;
            this.barriosRepository = barriosRepository;
            this.localidadesRepository = localidadesRepository;
            this.regionesRepository = regionesRepository;
        }

        public async Task<PagedResult> Get(long IdCliente, int? pageNumber, int? pageSize)
        {
            var sucursales = new List<Sucursal>();
            var t = Task.Run(() =>
            {
                sucursales = sucursalesRepository.Get(IdCliente, columnsToIgnore).Result.ToList();

                foreach (var sucursal in sucursales)
                {
                    if (sucursal.IdBarrio.HasValue)
                        sucursal.Barrio = (Barrio)barriosRepository.Get(sucursal.IdBarrio.Value).Result;

                    if (sucursal.IdProvincia.HasValue)
                        sucursal.Provincia = (Provincia)provinciasRepository.Get(sucursal.IdProvincia.Value).Result;

                    if (sucursal.IdLocalidad.HasValue)
                        sucursal.Localidad = (Localidad)localidadesRepository.Get(sucursal.IdLocalidad.Value).Result;

                    if (sucursal.IdZona.HasValue)
                        sucursal.Zona = (Zona)zonasRepository.Get(sucursal.IdZona.Value).Result;

                    if (sucursal.IdRegion.HasValue)
                        sucursal.Region = (Region)regionesRepository.Get(sucursal.IdRegion.Value).Result;

                }
            });
            t.Wait();

            var pagedList = new PagedList<Sucursal>(sucursales, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, sucursales.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public Task<Sucursal> Get(long id, long IdCliente)
        {
            var sucursales = new Sucursal();
            var t = Task.Run(() =>
            {
                sucursales = sucursalesRepository.Get(id, IdCliente, columnsToIgnore).Result;


            });
            t.Wait();

            return Task.FromResult(sucursales);
        }

        public async Task<PagedResult> Get(string name, long IdCliente, int? pageNumber, int? pageSize)
        {
            var sucursales = new List<Sucursal>();
            var t = Task.Run(() =>
            {
                sucursales = sucursalesRepository.Get(name, IdCliente,columnsToIgnore).Result.ToList();

                foreach (var sucursal in sucursales)
                {
                    if (sucursal.IdBarrio.HasValue)
                        sucursal.Barrio = (Barrio)barriosRepository.Get(sucursal.IdBarrio.Value).Result;

                    if (sucursal.IdProvincia.HasValue)
                        sucursal.Provincia = (Provincia)provinciasRepository.Get(sucursal.IdProvincia.Value).Result;

                    if (sucursal.IdLocalidad.HasValue)
                        sucursal.Localidad = (Localidad)localidadesRepository.Get(sucursal.IdLocalidad.Value).Result;

                    if (sucursal.IdZona.HasValue)
                        sucursal.Zona = (Zona)zonasRepository.Get(sucursal.IdZona.Value).Result;

                    if (sucursal.IdRegion.HasValue)
                        sucursal.Region = (Region)regionesRepository.Get(sucursal.IdRegion.Value).Result;

                }

            });
            t.Wait();



            var pagedList = new PagedList<Sucursal>(sucursales, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, sucursales.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override async Task<ServiceResult> Insert(Sucursal sucursal
            )
        {
            var result = new ServiceResult() { HasErrors = false };


            var t = Task.Run(() =>
            {


                sucursal.Creado = DateTime.Now;


                result.IdObjeto = sucursalesRepository.InsertAndGetId(sucursal, columnsToIgnore).Result;

                if (result.IdObjeto <= 0)
                    result = GetServiceResult(ServiceMethod.Insert, "Sucursal", false);





            });
            t.Wait();

            return result;
        }

        public override async Task<ServiceResult> Update(Sucursal sucursal
            )
        {
            var existe = (await sucursalesRepository.Get(sucursal.Id, sucursal.IdCliente, columnsToIgnore)).Id > 0;
            if (!existe)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Sucursal");



            return await base.Update(sucursal, columnsToIgnore);
        }

        public async Task<ServiceResult> Delete(long id)
        {
            return GetServiceResult(ServiceMethod.Delete, "Sucursales", await sucursalesRepository.Delete(new Sucursal() { Id = id, Modificado = DateTime.Now }));
        }


    }
}
