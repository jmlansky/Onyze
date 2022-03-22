using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Localidades;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Service.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    public class LocalidadesController : BaseController
    {
        private readonly ILocalidadesService localidadesService;
        public LocalidadesController(IConfiguration configuration, ILocalidadesService localidadesService): base(configuration)
        {
            this.localidadesService = localidadesService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await localidadesService.Get();
            var dto = new List<LocalidadDto>();
            foreach (var localidad in result)
                dto.Add(MapperEntidadDto.Mapper(localidad, new LocalidadDto()));

            return Ok(dto);
        }

        // GET <LocalidadesController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var localidad = await localidadesService.Get(id);
            if (localidad.Id == 0)
                return NotFound();
            var dto = MapperEntidadDto.Mapper(localidad, new LocalidadDto());

            return Ok(dto);
        }

        // POST <LocalidadesController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostLocalidadDto dto)
        {
            try
            {
                var localidad = MapperEntidadDto.Mapper(dto, new Localidad());
                var result = await localidadesService.Insert(localidad);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT <LocalidadesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutLocalidadDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");

                var localidad = MapperEntidadDto.Mapper(dto, new Localidad());
                localidad.Id = id;

                var result = await localidadesService.Update(localidad);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE <LocalidadesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");

                var result = await localidadesService.Delete(id);
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
