using EnviadorDeMail;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Totems;
using Touch.Core.Usuarios;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Sucursales.Contracts;
using Touch.Repositories.Totems;
using Touch.Repositories.Totems.Contracts;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Comun;
using Touch.Service.Totems;
using Touch.Service.Usuarios.Contracts;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Clientes
{
    public class TotemsService : SingleEntityComunService<Totem>, ITotemsService
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly IBarriosRepository barriosRepository;
        private readonly IProvinciasRepository provinciasRepository;
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly ISucursalesRepository sucursalesRepository;
        private readonly ITotemsRepository totemsRepository;
        private readonly ISingleEntityComunRepository<Sector> sectoresRepository;


        private string[] columnsToIgnore = { "Sucursal", "Sectores", "IdSectores" };

        public TotemsService(ISucursalesRepository sucursalesRepository, ITotemsRepository totemsRepository,
            ISingleEntityComunRepository<Sector> sectoresRepository

                ) : base(totemsRepository)
        {
            this.totemsRepository = totemsRepository;
            this.totemsRepository = totemsRepository;
            this.sectoresRepository = sectoresRepository;
            this.sucursalesRepository = sucursalesRepository;
        }

        public override async Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var totems = new List<Totem>();
            var t = Task.Run(() =>
            {
                totems = totemsRepository.Get(columnsToIgnore).Result.ToList();

                var barrios = barriosRepository.Get().Result;
                var localidades = localidadesRepository.Get().Result;
                var provincias = provinciasRepository.Get().Result;


                //var clientes = clientesRepository.Get().Result;

                //foreach (var usuario in usuarios)
                //    usuario.Cliente = clientes.FirstOrDefault(x => x.Id == usuario.IdCliente);
            });
            t.Wait();

            var pagedList = new PagedList<Totem>(totems, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, totems.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override Task<Totem> Get(long id)
        {
            var totems = new Totem();
            var t = Task.Run(() =>
            {
                totems = totemsRepository.Get(id, columnsToIgnore).Result;


            });
            t.Wait();

            return Task.FromResult(totems);
        }

        public   async Task<PagedResult> Get(string name, long idCliente, long? idSucursal, int? pageNumber, int? pageSize)
        {
            var totems = new List<Totem>();
            var t = Task.Run(() =>
            {
                totems = totemsRepository.Get(name, idCliente, idSucursal,columnsToIgnore).Result.ToList();
                foreach (var totem in totems)
                {
                    totem.Sectores = totemsRepository.GetSectoresFromTotem(totem.Id).ToList();

                    totem.Sucursal = sucursalesRepository.Get(totem.IdSucursal, idCliente).Result;


                }
            });
            t.Wait();



            var pagedList = new PagedList<Totem>(totems, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, totems.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override async Task<ServiceResult> Insert(Totem totem)
        {
            var result = new ServiceResult() { HasErrors = false };


            var t = Task.Run(() =>
            {


                totem.Creado = DateTime.Now;

                totem.Serial = Guid.NewGuid().ToString();

                result.IdObjeto = totemsRepository.InsertAndGetId(totem, columnsToIgnore).Result;

                if (result.IdObjeto <= 0)
                    result = GetServiceResult(ServiceMethod.Insert, "Cliente", false);





            });
            t.Wait();

            return result;
        }

        public override async Task<ServiceResult> Update(Totem totem)
        {
            var existe = (await totemsRepository.Get(totem.Id, columnsToIgnore)).Id > 0;
            if (!existe)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Totem");



            return await base.Update(totem, columnsToIgnore);
        }

        public async Task<ServiceResult> Delete(long id)
        {
            
            return GetServiceResult(ServiceMethod.Delete, "Totems", await totemsRepository.Delete(new Totem() { Id = id, Modificado = DateTime.Now }));

        }

        public async Task<PagedResult> GetFromSucursal(long idCliente, long? idSucursal, int? pageNumber, int? pageSize)
        {
            var totems = new List<Totem>();
            var t = Task.Run(() =>
            {
                totems = totemsRepository.GetFromSucursal(idCliente, idSucursal, columnsToIgnore).Result.ToList();

                foreach (var totem in totems)
                {
                    totem.Sectores = totemsRepository.GetSectoresFromTotem(totem.Id).ToList();

                    totem.Sucursal = sucursalesRepository.Get(totem.IdSucursal, idCliente).Result;


                }
            });
            t.Wait();

            var pagedList = new PagedList<Totem>(totems, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, totems.Count()) { PagedList = pagedList };

            return pagedResult;
        }
    }
}
