using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Zonas;

using Touch.Core.Comun;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
    public class ZonasController : BaseController
    {
        private readonly IZonasService zonasService;
        public ZonasController(IConfiguration configuration, IZonasService zonasService): base(configuration)
        {
            this.zonasService = zonasService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await zonasService.Get();
            var dto = new List<ZonaDto>();
            foreach (var zona in result)
                dto.Add(MapperEntidadDto.Mapper(zona, new ZonaDto()));

            return Ok(dto);
        }

        // GET <zonasController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var zona = await zonasService.Get(id);
            if (zona.Id == 0)
                return NotFound();

            var dto = MapperEntidadDto.Mapper(zona, new ZonaDto());

            return Ok(dto);
        }

        // POST <zonasController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostZonaDto dto)
        {
            try
            {
                var zona = MapperEntidadDto.Mapper(dto, new Zona());
                var result = await zonasService.Insert(zona);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT <zonasController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, [FromBody] PutZonaDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");

                var zona = MapperEntidadDto.Mapper(dto, new Zona());
                zona.Id = id;

                var result = await zonasService.Update(zona);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE <zonasController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");

                var result = await zonasService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
