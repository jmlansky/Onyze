using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Comun.Dtos.Responses;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagedList;
using Touch.Api.Dtos.Barrios;
using Touch.Api.Dtos.Cliente;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Usuarios;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Comun;
using Touch.Service.Usuarios;

namespace Touch.Api.Controllers
{
    public class ClientesController : BaseController
    {
        private readonly IClientesService clientesService;

        public ClientesController(IConfiguration configuration, IClientesService clientesService) : base(configuration)
        {
            this.clientesService = clientesService;

        }

        // GET: <BarriosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {


            var result = await clientesService.Get(pageNumber, pageSize);



            var response = new PagedResponse<ClienteDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Cliente>)result.PagedList).Select(x => MapearClientesDto(x)).ToList();


            return Ok(response);


        }

        private ClienteDto MapearClientesDto(Cliente item)
        {

            var dtoItem = MapperEntidadDto.Mapper(item, new ClienteDto());


            if (item.Barrio != null)
                dtoItem.Barrio = MapperEntidadDto.Mapper(item.Barrio, new BarrioDto());

            if (item.Localidad != null)
                dtoItem.Localidad = MapperEntidadDto.Mapper(item.Localidad, new LocalidadDto());

            if (item.Provincia != null)
                dtoItem.Provincia = MapperEntidadDto.Mapper(item.Provincia, new ProvinciaDto());

            if (item.Usuarios.Any())
            {



                foreach (var usuario in item.Usuarios)
                {
                    var usu = MapperEntidadDto.Mapper(usuario, new GetUsuarioDto());

                    if (usuario.Rol != null)
                        usu.rol = usuario.Rol.Nombre;

                    dtoItem.Usuarios.Add(usu);
                }

            }


            return dtoItem;
        }

        // GET <BarriosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var result = await clientesService.Get(id);
            if (result != null && result.Id > 0)
                return Ok(MapearClientesDto(result));
            return NotFound();
        }

        // GET <BarriosController>/nombre
        [HttpGet("Nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get(string nombre, int? pageNumber, int? pageSize)
        {
            var result = await clientesService.Get(nombre, pageNumber, pageSize);
            var response = new PagedResponse<ClienteDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Cliente>)result.PagedList).Select(x => MapearClientesDto(x)).ToList();
            return Ok(response);
        }


       

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostClienteDto dto)
        {


            var cliente = MapperEntidadDto.Mapper(dto, new Cliente());

            if (cliente.idLocalidad == 0)
                cliente.idLocalidad = null;
            if (cliente.IdBarrio == 0)
                cliente.IdBarrio = null;
            if (cliente.idProvincia == 0)
                cliente.idProvincia = null;


            var result = await clientesService.Insert(cliente);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <BarriosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, PostClienteDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var cliente = MapperEntidadDto.Mapper(dto, new Cliente());
            cliente.Id = id;
            var result = await clientesService.Update(cliente);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // DELETE <BarriosController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");
            var result = await clientesService.Delete(id);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
