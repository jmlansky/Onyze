using Framework.Helpers;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Api.Dtos.Usuarios;
using Touch.Core.Usuarios;
using Touch.Service.Usuarios.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System;
using Touch.Api.Dtos.Cliente;

namespace Touch.Api.Controllers
{
    public class UsuariosController : BaseController
    {
        private readonly IUsuariosService usuariosService;
        public UsuariosController(IConfiguration configuration, IUsuariosService usuariosService) : base(configuration)
        {
            this.usuariosService = usuariosService;
        }

        // GET: <UsuariosController>
        [HttpGet]
        [Authorize(Roles = "Super")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await usuariosService.Get(pageNumber, pageSize);

            var response = new PagedResponse<GetUsuarioDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords,
                List = MapearUsuarioDto((PagedList<Usuario>)result.PagedList)
            };
            return Ok(response);
        }

        // GET <UsuariosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get(int id)
        {
            //Todo: ver que el admin y super puedan ver todo, guest y usuarios, solo su propios datos

            var result = await usuariosService.Get(id);
            var dto = MapperEntidadDto.Mapper(result, new GetUsuarioDto());
            dto.Cliente =result.Cliente.Nombre;

            return Ok(dto);
        }

        [HttpGet("Filtrado")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get([Required] long idCliente, string nombre, int? pageNumber, int? pageSize)
        {
            var result = await usuariosService.GetFromCliente(idCliente, nombre, pageNumber, pageSize);

            var response = new PagedResponse<GetUsuarioDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords,
                List = MapearUsuarioDto((PagedList<Usuario>)result.PagedList)
            };
            return Ok(response);
        }

        // POST <UsuariosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostUsuarioDto dto)
        {
            if (string.IsNullOrEmpty(dto.Mail))
                return BadRequest("por favor ingrese el mail del usuario");

            if (string.IsNullOrEmpty(dto.Nombre))
                return BadRequest("por favor ingrese el nombre y apellido del usuario");

            var usuario = MapperEntidadDto.Mapper(dto, new Usuario());
            var result = await usuariosService.Insert(usuario);
            return StatusCode((int)result.StatusCode, result);
        }

        // POST <UsuariosController>
        [HttpPost("Usuarios/PasswordReset")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PasswordReset([FromBody] PostUsuarioDto dto)
        {
            throw new NotImplementedException();

            if (string.IsNullOrEmpty(dto.Mail))
                return BadRequest("por favor ingrese el mail del usuario");

            if (string.IsNullOrEmpty(dto.Nombre))
                return BadRequest("por favor ingrese el nombre y apellido del usuario");

            var usuario = MapperEntidadDto.Mapper(dto, new Usuario());
            var result = await usuariosService.Insert(usuario);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT <UsuariosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, [FromBody] PutUsuarioDto dto)
        {

            //Todo: ver que el admin y super puedan cambiar todo, guest y usuarios, solo su propios datos

            if (id <= 0)
                return BadRequest("por favor ingrese un id válido");

            if (string.IsNullOrEmpty(dto.Nombre))
                return BadRequest("por favor ingrese el nombre y apellido del usuario");

            var usuario = MapperEntidadDto.Mapper(dto, new Usuario());
            usuario.Id = id;
            var result = await usuariosService.Update(usuario);
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT <UsuariosController>/5/Password
        [HttpPut("PasswordUpdate")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PasswordUpdate([FromBody] PutCambiarPasswordDto dto)
        {

            //Todo: ver que el admin y super puedan cambiar todo, guest y usuarios, solo su propia pass
            if (string.IsNullOrEmpty(dto.NombreUsuario))
                return BadRequest("por favor ingrese un nombre de usuario válido");

            if (string.IsNullOrEmpty(dto.PasswordViejo) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.ConfirmacionPassword))
                return BadRequest("Por favor ingrese los 3 campos.");

            if (!dto.Password.Equals(dto.ConfirmacionPassword))
                return BadRequest("El password nuevo y la confirmación del mismo no coinciden.");

            var usuario = MapperEntidadDto.Mapper(dto, new Usuario());

            var result = await usuariosService.UpdatePassword(usuario);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE <UsuariosController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("por favor ingrese un id válido");

            var result = await usuariosService.Delete(new Usuario() { Id = id });
            return StatusCode((int)result.StatusCode, result);
        }


        private List<GetUsuarioDto> MapearUsuarioDto(PagedList<Usuario> pagedListUsuarios)
        {
            var list = new List<GetUsuarioDto>();
            foreach (var usuarioPaginado in pagedListUsuarios)
            {
                var usuarioDto = MapperEntidadDto.Mapper(usuarioPaginado, new GetUsuarioDto());
                if (usuarioPaginado.Cliente != null)
                    usuarioDto.Cliente =usuarioPaginado.Cliente.Nombre;

                if (usuarioPaginado.Rol != null)
                    usuarioDto.rol = usuarioPaginado.IdRol.ToString();

                list.Add(usuarioDto);
            }

            return list;
        }
    }
}
