using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.TiposObjetoPublicitar;

using Touch.Core.Pubilicidades;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
     public class TiposObjetoPublicitarController : BaseController
    {
        private readonly ISingleEntityComunService<TipoObjetoPublicitar> tipoObjetoPublicitarService;
        public TiposObjetoPublicitarController(IConfiguration configuration, ISingleEntityComunService<TipoObjetoPublicitar> tipoObjetoPublicitarService): base(configuration)
        {
            this.tipoObjetoPublicitarService = tipoObjetoPublicitarService;
        }

        // Delete <TiposObjetoPublicitarController>/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var archivo = new TipoObjetoPublicitar()
            {
                Id = id,
                Modificado = DateTime.Now
            };

            var result = await tipoObjetoPublicitarService.Delete(archivo);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        // GET<TiposObjetoPublicitarController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await tipoObjetoPublicitarService.Get();
            var dto = new List<TipoObjetoPublicitarDto>();
            if (result.Any())
            {
                foreach (var sector in result)
                    dto.Add(MapperEntidadDto.Mapper(sector, new TipoObjetoPublicitarDto()));
            }

            return Ok(dto);
        }

        // GET<TiposObjetoPublicitarController>/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await tipoObjetoPublicitarService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            var dto = new TipoObjetoPublicitarDto();

            if (result != null && result.Id > 0)
                dto = MapperEntidadDto.Mapper(result, new TipoObjetoPublicitarDto());

            return Ok(dto);
        }

        // GET<TiposObjetoPublicitarController>/nombre/{nombre}
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Por favor ingrese un nombre válido");

            var result = await tipoObjetoPublicitarService.Get(nombre);
            var dto = new List<TipoObjetoPublicitarDto>();
            if (result.Any())
            {
                foreach (var sector in result)
                    dto.Add(MapperEntidadDto.Mapper(sector, new TipoObjetoPublicitarDto()));
            }

            return Ok(dto);
        }

        // POST <SectorsController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostTipoObjetoPublicitarDto dto)
        {
            var tipo = MapperEntidadDto.Mapper(dto, new TipoObjetoPublicitar());
            tipo.Creado = DateTime.Now;

            tipo.Tags += ", " + dto.Nombre;
            var result = await tipoObjetoPublicitarService.Insert(tipo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <TiposObjetoPublicitarController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, PutTipoObjetoPublicitarDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var tipo = MapperEntidadDto.Mapper(dto, new TipoObjetoPublicitar());
            tipo.Id = id;
            tipo.Modificado = DateTime.Now;
            tipo.Tags += ", " + dto.Nombre;

            var result = await tipoObjetoPublicitarService.Update(tipo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
