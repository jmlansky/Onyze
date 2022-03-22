using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Framework.Auth.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Touch.Core.Auth;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Usuarios.Contracts;

namespace Framework.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuariosRepository usuariosRepository;
        private readonly IRolesRepository rolesRepository;
        private readonly IClientesRepository clientesRepository;

        public AuthService(IUsuariosRepository usuariosRepository,
            IRolesRepository rolesRepository, IClientesRepository clientesRepository)
        {
            this.usuariosRepository = usuariosRepository;
            this.rolesRepository = rolesRepository;
            this.clientesRepository = clientesRepository;
        }

        public async Task<LoginModel> Authenticate(LoginModel login)
        {
            var loginModel = new LoginModel()
            {
                NombreUsuario = login.NombreUsuario,
                Password = login.Password
            };
            var t = Task.Run(() =>
            {
                var usuario = usuariosRepository.Get(login.NombreUsuario, login.Password, new string[] { "Sucursal", "Rol", "Cliente", "PasswordViejo" }).Result;
                var rol = rolesRepository.Get(usuario.IdRol, new string[] { "Permisos" }).Result;

                var cliente = clientesRepository.Get(usuario.IdCliente, new string[] { "Barrio", "Localidad", "Provincia", "Usuarios", "Sucursales" }).Result;
                usuario.Cliente = cliente;
                loginModel.Usuario = usuario;
                loginModel.Rol = rol.Nombre;
                loginModel.Autenticado = usuario.Id > 0;
            });
            t.Wait();

            return loginModel;
        }

        public Task<string> GenerateToken(string key, string rolUsuario, long lifeTimeInSeconds)
        {
            //security key
            var securityKey = key;

            //symmetric security key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey.ToString()));

            //credentials
            var credentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //Add claims
            var claims = new List<Claim>()
            {
                //new Claim(ClaimTypes.Role, <<Aca se agrega el sol del usuario>>)
                new Claim(ClaimTypes.Role, rolUsuario),
                new Claim("idEmpresa", "1234"), //--> validar el id de empresa
            };

            //create token            
            var token = new JwtSecurityToken(
                issuer: "jmlansky",
                audience: "readers",
                expires: DateTime.Now.AddSeconds(lifeTimeInSeconds),
                signingCredentials: credentials,
                claims: claims
            );

            return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
