using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Sectores;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Service.Comun;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
     public class SectoresController : BaseController
    {
        private readonly ISingleEntityComunService<Sector> sectoresService;
        public SectoresController(IConfiguration configuration, ISingleEntityComunService<Sector> sectoresService): base(configuration)
        {
            this.sectoresService = sectoresService;
        }

        // Delete <SectoresController>/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var archivo = new Sector()
            {
                Id = id,
                Modificado = DateTime.Now
            };

            var result = await sectoresService.Delete(archivo);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        // GET<SectoresController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await sectoresService.Get();
            var dto = new List<SectorDto>();
            if (result.Any()) 
            {
                foreach (var sector in result)                
                    dto.Add(MapperEntidadDto.Mapper(sector, new SectorDto()));                
            }

            return Ok(dto);
        }

        // GET<SectoresController>/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await sectoresService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            var dto = new SectorDto();

            if (result != null && result.Id > 0)
                dto = MapperEntidadDto.Mapper(result, new SectorDto());            

            return Ok(dto);
        }

        // GET<SectoresController>/nombre/{nombre}
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                return BadRequest("Por favor ingrese un nombre válido");

            var result = await sectoresService.Get(nombre);
            var dto = new List<SectorDto>();
            if (result.Any())
            {
                foreach (var sector in result)
                    dto.Add(MapperEntidadDto.Mapper(sector, new SectorDto()));
            }

            return Ok(dto);
        }

        // POST <SectorsController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostSectorDto dto)
        {
            var sector = MapperEntidadDto.Mapper(dto, new Sector());
            sector.Creado = DateTime.Now;

            var result = await sectoresService.Insert(sector);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <SectoresController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, [FromBody] PutSectorDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var sector = MapperEntidadDto.Mapper(dto, new Sector());
            sector.Id = id;
            sector.Modificado = DateTime.Now;

            var result = await sectoresService.Update(sector);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

    }
}
