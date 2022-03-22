using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Barrios;
using Touch.Core.Comun;
using Touch.Service.Comun;


namespace Touch.Api.Controllers
{    
    public class BarriosController : BaseController
    {
        private readonly IBarriosService barriosService;
        public BarriosController(IConfiguration configuration, IBarriosService barriosService): base(configuration)
        {
            this.barriosService = barriosService;
        }

        // GET: <BarriosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await barriosService.Get();
            var dto = new List<BarrioDto>();
            if (result.Any())
            {
                foreach (var barrio in result)                
                    dto.Add(MapperEntidadDto.Mapper(barrio, new BarrioDto()));                
            }

            return Ok(dto);
        }

        // GET <BarriosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var result = await barriosService.Get(id);
            if (result != null && result.Id > 0)
                return Ok(MapperEntidadDto.Mapper(result, new BarrioDto()));
            return NotFound();
        }

        // GET <BarriosController>/nombre
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre)
        {
            var result = await barriosService.Get(nombre);
            var dto = new List<BarrioDto>();
            if (result.Any())
            {
                foreach (var barrio in result)
                    dto.Add(MapperEntidadDto.Mapper(barrio, new BarrioDto()));
            }

            return Ok(dto);
        }


        // POST <BarriosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostBarrioDto dto)
        {
            var barrio = MapperEntidadDto.Mapper(dto, new Barrio());
            var result = await barriosService.Insert(barrio);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <BarriosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, PutBarrioDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var barrio = MapperEntidadDto.Mapper(dto, new Barrio());
            barrio.Id = id;
            var result = await barriosService.Update(barrio);
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
            var result = await barriosService.Delete(id);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
