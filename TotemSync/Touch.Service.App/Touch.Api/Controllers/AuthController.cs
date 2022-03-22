using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Auth;
using Framework.Auth.Services.Contracts;
using Framework.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Controllers;
using Touch.Api.Dtos.Auth;
using Touch.Api.Dtos.Cliente;
using Touch.Api.Dtos.Usuarios;
using Touch.Core.Auth;
using Touch.Service.Usuarios.Contracts;

namespace framework.Auth.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IConfiguration configuration;
        public readonly IAuthService auhorizationService;

        public AuthController(IConfiguration configuration, IAuthService auhorizationService) : base(configuration)
        {
            this.auhorizationService = auhorizationService;
            this.configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            IActionResult response = Unauthorized();
            var usr = await AuthenticateUser(dto);
            if (usr.Autenticado)
            {
                var encryptedUser = Encryption.Encrypt(usr.NombreUsuario + ";" + usr.Rol);
                try
                {
                    var existeSesionActiva = ActiveUsers.SesionesActivas.Any(x => x.Key.Equals(encryptedUser));
                    if (!existeSesionActiva)
                        ActiveUsers.SesionesActivas.Add(encryptedUser, string.Empty);


                    //HttpContext.Session.SetString("usuario", usuario);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                var usuario = MapperEntidadDto.Mapper(usr.Usuario, new GetUsuarioDto());

                usuario.Cliente = usr.Usuario.Cliente.Nombre;

                usuario.rol = usr.Usuario.IdRol.ToString();

                response = Ok(new LoginResponse() { Messsage = "Success", NombreUsuario = usr.NombreUsuario, Rol = encryptedUser, Usuario = usuario });
                //var name = HttpContext.Session.GetString("usuario");
            }
            return response;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IEnumerable<string>> Get()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                return new string[] { accessToken };
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<LoginModel> AuthenticateUser(LoginDto dto)
        {
            var loginModel = MapperEntidadDto.Mapper(dto, new LoginModel());

            //Validate the User Credentials     
            loginModel = await auhorizationService.Authenticate(loginModel);
            if (!loginModel.Autenticado)
                HttpContext.Response.Headers.Remove("site");
            return loginModel;
        }
    }
}