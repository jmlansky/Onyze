using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
using Touch.Api.Dtos.Regiones;
using Touch.Api.Dtos.Sucursales;
using Touch.Api.Dtos.Usuarios;
using Touch.Api.Dtos.Zonas;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Comun;
using Touch.Service.Sucursales;
using Touch.Service.Usuarios;

namespace Touch.Api.Controllers
{
    public class SucursalesController : BaseController
    {
        private readonly ISucursalesService sucursalesService;

        public SucursalesController(IConfiguration configuration, ISucursalesService sucursalesService) : base(configuration)
        {
            this.sucursalesService = sucursalesService;

        }

        // GET: <BarriosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get([Required] long idCliente, int? pageNumber, int? pageSize)
        {


            var result = await sucursalesService.Get(idCliente, pageNumber, pageSize);



            var response = new PagedResponse<SucursalDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Sucursal>)result.PagedList).Select(x => MapeaSucursalDto(x)).ToList();


            return Ok(response);


        }

        private SucursalDto MapeaSucursalDto(Sucursal item)
        {

            var dtoItem = MapperEntidadDto.Mapper(item, new SucursalDto());


            if (item.Barrio != null)
                dtoItem.Barrio = MapperEntidadDto.Mapper(item.Barrio, new BarrioDto());

            if (item.Localidad != null)
                dtoItem.Localidad = MapperEntidadDto.Mapper(item.Localidad, new LocalidadDto());

            if (item.Provincia != null)
                dtoItem.Provincia = MapperEntidadDto.Mapper(item.Provincia, new ProvinciaDto());

            if (item.Zona != null)
                dtoItem.Zona= MapperEntidadDto.Mapper(item.Zona, new ZonaDto());

            if (item.Region != null)
                dtoItem.Region = MapperEntidadDto.Mapper(item.Region, new RegionDto());

            return dtoItem;
        }

        // GET <BarriosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get([Required] long id, [Required] long idCliente)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var result = await sucursalesService.Get(id, idCliente);
            if (result != null && result.Id > 0)
                return Ok(MapeaSucursalDto(result));
            return NotFound();
        }

        // GET <BarriosController>/nombre
        [HttpGet("Filtrado")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Filtrado(string nombre, [Required] long idCliente, int? pageNumber, int? pageSize)
        {
            var result = await sucursalesService.Get(nombre, idCliente, pageNumber, pageSize);
            var response = new PagedResponse<SucursalDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Sucursal>)result.PagedList).Select(x => MapeaSucursalDto(x)).ToList();
            return Ok(response);
        }



        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostSucursalDto dto)
        {
            var sucursal = MapperEntidadDto.Mapper(dto, new Sucursal());

            if (sucursal.IdBarrio == 0)
                sucursal.IdBarrio = null;

            if (sucursal.IdLocalidad == 0)
                sucursal.IdLocalidad = null;

            if (sucursal.IdProvincia == 0)
                sucursal.IdProvincia = null;

            if (sucursal.IdRegion == 0)
                sucursal.IdRegion = null;

            if (sucursal.IdZona == 0)
                sucursal.IdZona = null;

            var result = await sucursalesService.Insert(sucursal);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <BarriosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, PostSucursalDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var sucursal = MapperEntidadDto.Mapper(dto, new Sucursal());

            if (sucursal.IdBarrio == 0)
                sucursal.IdBarrio = null;

            if (sucursal.IdLocalidad == 0)
                sucursal.IdLocalidad = null;

            if (sucursal.IdProvincia == 0)
                sucursal.IdProvincia = null;

            if (sucursal.IdRegion == 0)
                sucursal.IdRegion = null;

            if (sucursal.IdZona == 0)
                sucursal.IdZona = null;

            sucursal.Id = id;
            var result = await sucursalesService.Update(sucursal);
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
            var result = await sucursalesService.Delete(id);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
