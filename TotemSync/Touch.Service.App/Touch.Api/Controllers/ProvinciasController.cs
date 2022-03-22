using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Provincias;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Service.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Localidades;

namespace Touch.Api.Controllers
{
      public class ProvinciasController : BaseController
    {
        private readonly IProvinciasService provinciasService;
        public ProvinciasController(IConfiguration configuration, IProvinciasService provinciasService): base(configuration)
        {
            this.provinciasService = provinciasService;
        }

        // GET: <ProvinciasController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await provinciasService.Get();
            var dto = new List<ProvinciaDto>();
            foreach (var provincia in result)
                dto.Add(MapperEntidadDto.Mapper(provincia, new ProvinciaDto()));

            return Ok(dto);
        }

        // GET <ProvinciasController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var provincia = await provinciasService.Get(id);
            if (provincia.Id == 0)
                return NotFound();

            var dto = MapperEntidadDto.Mapper(provincia, new ProvinciaDto());

            return Ok(dto);
        }

        // GET <ProvinciasController>/5
        [HttpGet("{id}/Localidades")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Localidades(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id de provincia válido.");

            var result = await provinciasService.GetLocalidades(id);
            var dto = new List<LocalidadDto>();
            foreach (var localidad in result)
                dto.Add(MapperEntidadDto.Mapper(localidad, new LocalidadDto()));

            return Ok(dto);
        }

        // POST <ProvinciasController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostProvinciaDto dto)
        {
            try
            {
                var provincia = MapperEntidadDto.Mapper(dto, new Provincia());
                var result = await provinciasService.Insert(provincia);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT <ProvinciasController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, [FromBody] PutProvinciaDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");

                var provincia = MapperEntidadDto.Mapper(dto, new Provincia());
                provincia.Id = id;

                var result = await provinciasService.Update(provincia);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE <ProvinciasController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido.");
                var result = await provinciasService.Delete(id);
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
