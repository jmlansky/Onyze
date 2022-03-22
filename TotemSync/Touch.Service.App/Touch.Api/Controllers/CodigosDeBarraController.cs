using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.CodigosDeBarra;
using Framework.Helpers;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    public class CodigosDeBarraController : BaseController
    {
        private readonly ICodigosDeBarraService codigosService;
        public CodigosDeBarraController(IConfiguration configuration, ICodigosDeBarraService codigosService): base(configuration)
        {
            this.codigosService = codigosService;
        }


        // Delete <CodigosDeBarraController>/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var codigo = new CodigoDeBarras()
            {
                Id = id,
                Modificado = DateTime.Now
            };

            var result = await codigosService.Delete(codigo);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        // GET<CodigosDeBarraController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await codigosService.Get();
            var dto = new List<CodigoDeBarrasDto>();
            if (result.Any())
            {
                foreach (var codigo in result)
                    dto.Add(MapperEntidadDto.Mapper(codigo, new CodigoDeBarrasDto()));
            }

            return Ok(dto);
        }

        // GET<CodigosDeBarraController>/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await codigosService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            var dto = new CodigoDeBarrasDto();

            if (result != null && result.Id > 0)
                dto = MapperEntidadDto.Mapper(result, new CodigoDeBarrasDto());

            return Ok(dto);
        }

        // GET<CodigosDeBarraController>/ean/{ean}
        [HttpGet("ean/{ean}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string ean)
        {
            if (string.IsNullOrWhiteSpace(ean))
                return BadRequest("Por favor ingrese un nombre válido");

            var result = await codigosService.Get(ean);
            var dto = new List<CodigoDeBarrasDto>();
            if (result.Any())
            {
                foreach (var codigo in result)
                    dto.Add(MapperEntidadDto.Mapper(codigo, new CodigoDeBarrasDto()));
            }

            return Ok(dto);
        }

        // POST <codigosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostCodigoDeBarrasDto dto)
        {
            var codigo = MapperEntidadDto.Mapper(dto, new CodigoDeBarras());
            codigo.Creado = DateTime.Now;
            var result = await codigosService.Insert(codigo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <CodigosDeBarraController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, PutCodigoDeBarrasDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var codigo = MapperEntidadDto.Mapper(dto, new CodigoDeBarras());
            codigo.Id = id;
            codigo.Modificado = DateTime.Now;
            var result = await codigosService.Update(codigo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
