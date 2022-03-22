using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Regiones;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Service.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    public class RegionesController : BaseController
    {
        private readonly IRegionesService regionesService;
        public RegionesController(IConfiguration configuration, IRegionesService regionesService): base(configuration)
        {
            this.regionesService = regionesService;
        }

        // GET: api/<RegionController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await regionesService.Get();
            var dto = new List<RegionDto>();
            if (result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new RegionDto())).ToList();

            return Ok(dto);
        }

        // GET api/<RegionController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await regionesService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            var dto = MapperEntidadDto.Mapper(result, new RegionDto());

            return Ok(dto);
        }

        [HttpGet("Nombre /{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre)
        {
            var result = await regionesService.Get(nombre);
            var dto = new List<RegionDto>();
            if (result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new RegionDto())).ToList();

            return Ok(dto);
        }

        // POST api/<RegionController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostRegionDto dto)
        {
            var result = await regionesService.InsertAndGetId(MapperEntidadDto.Mapper(dto, new Region()));
            return StatusCode((int)result.StatusCode, result);
        }

        // PUT api/<RegionController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id,  [FromBody] PutRegionDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var region = MapperEntidadDto.Mapper(dto, new Region());
            region.Id = id;
            var result = await regionesService.Update(region);
            return StatusCode((int)result.StatusCode, result);
        }

        // DELETE api/<RegionController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await regionesService.Delete(new Region() { Id = id });
            return StatusCode((int)result.StatusCode, result);
        }
    }
}
