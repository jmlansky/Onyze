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
using Touch.Core.Usuarios;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Comun;
using Touch.Service.Usuarios.Contracts;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Clientes
{
    public class ClientesService : SingleEntityComunService<Cliente>, IClientesService
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly IBarriosRepository barriosRepository;
        private readonly IProvinciasRepository provinciasRepository;
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly IClientesRepository clientesRepository;
        private readonly IRolesRepository rolesRepository;

        private string[] columnsToIgnore = { "Cliente", "Sucursal", "Provincia", "Localidad", "Barrio", "Usuarios", "Sucursales"};

        public ClientesService(IClientesRepository clientesRepository, IUsuariosRepository usuariosRepository,
            IBarriosRepository barriosRepository, IProvinciasRepository provinciasRepository, ILocalidadesRepository localidadesRepository ,IRolesRepository rolesRepository
                ) : base(clientesRepository)
        {
            this.usuariosRepository = usuariosRepository;
            this.clientesRepository = clientesRepository;
            this.barriosRepository = barriosRepository;
            this.localidadesRepository = localidadesRepository;
            this.provinciasRepository = provinciasRepository;
            this.rolesRepository = rolesRepository;
        }

        public override async Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var clientes = new List<Cliente>();

            clientes = clientes.OrderBy(o => o.Codigo).ToList();


            var t = Task.Run(() =>
            {
                clientes = clientesRepository.Get(columnsToIgnore).Result.ToList();

                var barrios = barriosRepository.Get().Result;
                var localidades = localidadesRepository.Get().Result;
                var provincias = provinciasRepository.Get().Result;

                foreach (var cliente in clientes)
                {
                    cliente.Barrio = (Barrio)barrios.FirstOrDefault(x => x.Id == cliente.IdBarrio);
                    cliente.Localidad = (Localidad)localidades.FirstOrDefault(x => x.Id == cliente.idLocalidad);
                    cliente.Provincia = (Provincia)provincias.FirstOrDefault(x => x.Id == cliente.idProvincia);
                }

                //var clientes = clientesRepository.Get().Result;

                //foreach (var usuario in usuarios)
                //    usuario.Cliente = clientes.FirstOrDefault(x => x.Id == usuario.IdCliente);
            });
            t.Wait();

            var pagedList = new PagedList<Cliente>(clientes, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, clientes.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override Task<Cliente> Get(long id)
        {
            var Cliente = new Cliente();
            var t = Task.Run(() =>
            {
                Cliente = clientesRepository.Get(id, columnsToIgnore).Result;


                if (Cliente.IdBarrio.HasValue)
                    Cliente.Barrio = (Barrio)barriosRepository.Get(Cliente.IdBarrio.Value).Result;

                if (Cliente.idLocalidad.HasValue)
                    Cliente.Localidad = (Localidad)localidadesRepository.Get(Cliente.idLocalidad.Value).Result;

                if (Cliente.idProvincia.HasValue)
                    Cliente.Provincia = (Provincia)provinciasRepository.Get(Cliente.idProvincia.Value).Result;


                Cliente.Usuarios = usuariosRepository.GetFromCliente(id,new string[] { "Rol","Sucursal","PasswordViejo","Cliente" }).Result.ToList();

                if (Cliente.Usuarios.Any())
                {
                    var roles = rolesRepository.Get(new string[] { "Permisos" }).Result;

                    foreach (var item in Cliente.Usuarios)
                    {
                        item.Rol = roles.FirstOrDefault(x => x.Id == item.IdRol);
                    }

                }

                //usuario.Cliente = clientesRepository.Get(usuario.IdCliente).Result;
            });
            t.Wait();

            return Task.FromResult(Cliente);
        }

        public override async Task<PagedResult> Get(string name, int? pageNumber, int? pageSize)
        {
            var usuarios = new List<Usuario>();
            var clientes = new List<Cliente>();
            var t = Task.Run(() =>
            {
                usuarios = usuariosRepository.Get(new string[] { "PasswordViejo", "Rol", "Cliente", "Sucursal" }).Result.ToList();
                clientes = clientesRepository.Get(name).Result.ToList();

                var barrios = barriosRepository.Get().Result;
                var localidades = localidadesRepository.Get().Result;
                var provincias = provinciasRepository.Get().Result;

                foreach (var cliente in clientes)
                {
                    cliente.Barrio = (Barrio)barrios.FirstOrDefault(x => x.Id == cliente.IdBarrio);
                    cliente.Localidad = (Localidad)localidades.FirstOrDefault(x => x.Id == cliente.idLocalidad);
                    cliente.Provincia = (Provincia)provincias.FirstOrDefault(x => x.Id == cliente.idProvincia);

                    var usu = usuarios.Where(x => x.IdCliente == cliente.Id);
                    if (usu.Any())
                    {
                        cliente.Usuarios = new List<Usuario>();
                        cliente.Usuarios.AddRange(usu);

                    }

                }
            });
            t.Wait();



            var pagedList = new PagedList<Cliente>(clientes, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, clientes.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override async Task<ServiceResult> Insert(Cliente cliente)
        {
            var result = new ServiceResult() { HasErrors = false };

            var clientes = clientesRepository.Get().Result;
            var existe = (clientes.FirstOrDefault(x => x.Codigo == cliente.Codigo)) != null;
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Cliente");


            var t = Task.Run(() =>
            {


                cliente.Creado = DateTime.Now;


                result.IdObjeto = clientesRepository.InsertAndGetId(cliente, columnsToIgnore).Result;

                if (result.IdObjeto <= 0)
                    result = GetServiceResult(ServiceMethod.Insert, "Cliente", false);





            });
            t.Wait();

            return result;
        }

        public override async Task<ServiceResult> Update(Cliente cliente)
        {
            var existe = (await clientesRepository.Get(cliente.Id, columnsToIgnore)).Id > 0;
            if (!existe)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Cliente");



            return await base.Update(cliente, columnsToIgnore);
        }

        public async Task<ServiceResult> Delete(long id)
        {
            return GetServiceResult(ServiceMethod.Delete, "Clientes", await clientesRepository.Delete(new Cliente() { Id = id, Modificado = DateTime.Now }));
        }

     
    }
}
