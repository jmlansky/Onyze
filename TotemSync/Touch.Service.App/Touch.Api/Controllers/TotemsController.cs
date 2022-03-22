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
using Touch.Api.Dtos.Sectores;
using Touch.Api.Dtos.Sucursales;
using Touch.Api.Dtos.Totems;
using Touch.Api.Dtos.Usuarios;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Totems;
using Touch.Repositories.Usuarios.Contracts;
using Touch.Service.Clientes;
using Touch.Service.Comun;
using Touch.Service.Totems;
using Touch.Service.Usuarios;

namespace Touch.Api.Controllers
{
    public class TotemsController : BaseController
    {
         private readonly ITotemsService totemsService;

        public TotemsController(IConfiguration configuration,  ITotemsService totemsService) : base(configuration)
        {
         
            this.totemsService = totemsService;

        }

        // GET: <BarriosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get([Required] long idCliente, long? idSucursal, int? pageNumber, int? pageSize)
        {


            var result = await totemsService.GetFromSucursal(idCliente, idSucursal, pageNumber, pageSize);



            var response = new PagedResponse<TotemDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Totem>)result.PagedList).Select(x => MapeaTotemDto(x)).ToList();


            return Ok(response);


        }

        [HttpGet("/Totems/Nombre")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get([Required] string nombre,[Required] long idCliente, long? idSucursal, int? pageNumber, int? pageSize)
        {


            var result = await totemsService.Get(nombre,idCliente, idSucursal, pageNumber, pageSize);



            var response = new PagedResponse<TotemDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Totem>)result.PagedList).Select(x => MapeaTotemDto(x)).ToList();


            return Ok(response);


        }

        private TotemDto MapeaTotemDto(Totem item)
        {

            var dtoItem = MapperEntidadDto.Mapper(item, new TotemDto());


            //if (item.Sector != null)
            //    dtoItem.Sector = MapperEntidadDto.Mapper(item.Sector, new SectorDto());

            if (item.Sectores != null && item.Sectores.Any())
                foreach (var sector in item.Sectores)
                    dtoItem.Sectores.Add(MapperEntidadDto.Mapper(sector, new SectorDto()));


            if (item.Sucursal != null )
                    dtoItem.Sucursal =MapperEntidadDto.Mapper(item.Sucursal, new SucursalDto());

            return dtoItem;
        }

        // GET <BarriosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var result = await totemsService.Get(id);
            if (result != null && result.Id > 0)
                return Ok(MapeaTotemDto(result));
            return NotFound();
        }

       


        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostTotemDto dto)
        {
            var totem = MapperEntidadDto.Mapper(dto, new Totem());
            if (dto.IdSectores != null && dto.IdSectores.Any())
                foreach (var item in dto.IdSectores)
                    totem.IdSectores.Add(item);



            var result = await totemsService.Insert(totem);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <BarriosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, PutTotemDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var Totem = MapperEntidadDto.Mapper(dto, new Totem());
            Totem.Id = id;
            if (dto.IdSectores != null && dto.IdSectores.Any())
                foreach (var item in dto.IdSectores)
                    Totem.IdSectores.Add(item);

            var result = await totemsService.Update(Totem);
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
            var result = await totemsService.Delete(id);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
