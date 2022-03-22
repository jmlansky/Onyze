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
using Touch.Repositories.Comun;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Comun;
using Touch.Service.Usuarios.Contracts;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Usuarios
{
    public class UsuariosService : SingleEntityComunService<Usuario>, IUsuariosService
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly ISingleEntityComunRepository<Cliente> clientesRepository;
        private readonly IMailSender sender;

        private string[] columnsToIgnore = { "Cliente", "Sucursal", "Rol", "PasswordViejo" };

        public UsuariosService(IUsuariosRepository usuariosRepository,
                ISingleEntityComunRepository<Cliente> clientesRepository,
                IMailSender sender) : base(usuariosRepository)
        {
            this.usuariosRepository = usuariosRepository;
            this.clientesRepository = clientesRepository;
            this.sender = sender;
        }

        public override async Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var usuarios = new List<Usuario>();
            var t = Task.Run(() =>
            {
                usuarios = usuariosRepository.Get(columnsToIgnore).Result.ToList();
                var clientes = clientesRepository.Get().Result;

                foreach (var usuario in usuarios)
                    usuario.Cliente = clientes.FirstOrDefault(x => x.Id == usuario.IdCliente);
            });
            t.Wait();

            var pagedList = new PagedList<Usuario>(usuarios, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, usuarios.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override Task<Usuario> Get(long id)
        {
            var usuario = new Usuario();
            var t = Task.Run(() =>
            {
                usuario = usuariosRepository.Get(id, columnsToIgnore).Result;
                usuario.Cliente = clientesRepository.Get(usuario.IdCliente).Result;
            });
            t.Wait();

            return Task.FromResult(usuario);
        }

        public override async Task<PagedResult> Get(string name, int? pageNumber, int? pageSize)
        {
            var usuarios = new List<Usuario>();
            var t = Task.Run(() =>
            {
                usuarios = usuariosRepository.Get(name, columnsToIgnore).Result.ToList();
                var clientes = clientesRepository.Get().Result;

                foreach (var usuario in usuarios)
                    usuario.Cliente = clientes.FirstOrDefault(x => x.Id == usuario.IdCliente);
            });
            t.Wait();

            var pagedList = new PagedList<Usuario>(usuarios, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, usuarios.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public async Task<PagedResult> GetFromCliente(long idCliente, string name, int? pageNumber, int? pageSize)
        {
            var usuarios = new List<Usuario>();
            var t = Task.Run(() =>
            {
                usuarios = usuariosRepository.GetFromCliente(idCliente, name, columnsToIgnore).Result.ToList();
                //var clientes = clientesRepository.Get(columnsToIgnore).Result;

                foreach (var usuario in usuarios)
                    usuario.Rol = new Core.Auth.Rol() { Id = usuario.IdRol };
            });
            t.Wait();

            var pagedList = new PagedList<Usuario>(usuarios, pageNumber ?? 1, pageSize ?? 25);

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, usuarios.Count()) { PagedList = pagedList };

            return pagedResult;
        }

        public override async Task<ServiceResult> Insert(Usuario usuario)
        {
            var result = new ServiceResult() { HasErrors = false };
            var t = Task.Run(() =>
            {

                try
                {
                    var usuarios = usuariosRepository.Get(columnsToIgnore).Result.ToList();
                    var existe = usuarios.Where(x => x.NombreUsuario.Equals(usuario.NombreUsuario.Trim())).Any();

                    if (existe)
                        throw new Exception("Nombre de usuario existente");

                    existe = usuarios.Where(x => x.Mail != null).Where(x => x.Mail.Equals(usuario.Mail.Trim())).Any();
                    if (existe)
                        throw new Exception("E-mail de usuario existente");

                    if (clientesRepository.Get(usuario.IdCliente, new string[] { "Barrio", "Localidad", "Provincia", "Usuarios", "Sucursales" }).Result.Id == 0)
                        throw new Exception("Cliente inexistente");

                    if (!result.HasErrors)
                    {
                        //usuario.RequiereCambiarPassword = true;
                        var psw = GenerarPasswordAleatorio();
                        usuario.Password = GetEncryptedPassword(psw);

                        result = base.InsertAndGetId(usuario, columnsToIgnore).Result;
                        if (!result.HasErrors)
                        {
                            var mensaje = "Bienvenida/o a SimpliSales - Su nueva contraseña es: " + psw;
                            //mensaje += " - Recuerde que necesitará cambiar la misma cuando inicie sesión por primera vez";

                            var mailConfirmation = sender.Send(usuario.Mail.Trim(), "SimpliSales - confirmación de usuario", mensaje).Result;
                            if (!mailConfirmation.Success)
                                result.Message = "Se ha insertado el usuario, pero no se ha podido enviar el mail de password.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasErrors = true;
                    result.Message = "Error al insertar usuario";
                    result.Notes = ex.Message;
                    result.Method = "Insert";
                    result.StatusCode = ServiceMethodsStatusCode.Error;
                }

            });
            t.Wait();

            return result;
        }

        public override async Task<ServiceResult> Update(Usuario usuario)
        {


            var usuarioActual = await usuariosRepository.Get(usuario.Id, columnsToIgnore);
            var existe = usuarioActual.Id > 0;
            if (!existe)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Usuario");


            var usuarios = usuariosRepository.Get(columnsToIgnore).Result.ToList();

            existe = usuarios.Where(x => x.Id != usuarioActual.Id).Where(x => x.Mail != null).Where(x => x.Mail.Equals(usuario.Mail.Trim())).Any();
            if (existe)
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = "No se puede actualizar usuario.",
                    Notes = "E-mail de usuario existente",
                    Method = "Update",
                    StatusCode = ServiceMethodsStatusCode.Error
                };



            columnsToIgnore = columnsToIgnore.Append("Password").ToArray();
            columnsToIgnore = columnsToIgnore.Append("RequiereCambiarPassword").ToArray();

            return await base.Update(usuario, columnsToIgnore);
        }

        public async Task<ServiceResult> UpdatePassword(Usuario usuario)
        {
            try
            {
                var usuarioExistente = await usuariosRepository.GetPorNombreUsuario(usuario.NombreUsuario, columnsToIgnore, false);
                if (usuarioExistente.Id <= 0)
                    throw new Exception("El usuario no existe");

                if (!usuarioExistente.Password.Equals(usuario.PasswordViejo))
                    throw new Exception("El password que desea cambiar no coincide con el que tiene guardado");

                if (usuarioExistente.Password.Equals(usuario.Password))
                    throw new Exception("El password nuevo no debe ser igual al actual");

                return GetServiceResult(ServiceMethod.Update, "Usuario - Password", await usuariosRepository.UpdatePassword(usuario));
            }
            catch (Exception ex)
            {
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = "Usuario - Password",
                    Notes = ex.Message,
                    Method = "Update",
                    StatusCode = ServiceMethodsStatusCode.Error
                };
            }

        }

        private string GenerarPasswordAleatorio()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GetEncryptedPassword(string str)
        {
            SHA256 sha256 = SHA256.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();

            var sb = new StringBuilder();
            var stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
