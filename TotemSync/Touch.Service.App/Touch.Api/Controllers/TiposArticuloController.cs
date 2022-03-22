using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.TiposArticulos;
using Touch.Core.Articulos;
using Touch.Service.Articulos;


namespace Touch.Api.Controllers
{
    public class TiposArticuloController : BaseController
    {
        private readonly ITipoDeArticuloService tipoDeArticuloService;
        public TiposArticuloController(IConfiguration configuration, ITipoDeArticuloService tipoDeArticuloService): base(configuration)
        {
            this.tipoDeArticuloService = tipoDeArticuloService;
        }
        // GET: <TiposArticuloController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var tipos = await tipoDeArticuloService.Get();
            var dto = new List<TipoArticuloDto>();
            foreach (var tipo in tipos)
                dto.Add(MapperEntidadDto.Mapper(tipo, new TipoArticuloDto()));

            return Ok(dto);
        }

        // GET <TiposArticuloController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            var tipo = await tipoDeArticuloService.Get(id);
            if (tipo.Id.Equals(0))
                return NotFound();
            return Ok(MapperEntidadDto.Mapper(tipo, new TipoArticuloDto()));
        }

        // GET <TiposArticuloController>/nombre/"medicamento"
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre)
        {
            var dto = new List<TipoArticuloDto>();
            foreach (var tipo in await tipoDeArticuloService.Get(nombre))
                dto.Add(MapperEntidadDto.Mapper(tipo, new TipoArticuloDto()));

            return Ok(dto);
        }

        // POST <TiposArticuloController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostTipoArticuloDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var tipo = MapperEntidadDto.Mapper(dto, new TipoArticulo());

                var result = await tipoDeArticuloService.Insert(tipo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de artículo: "+ ex.Message);
            }
        }

        // PUT <TiposArticuloController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutTipoArticuloDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor seleccione un id válido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var tipo = MapperEntidadDto.Mapper(dto, new TipoArticulo());
                tipo.Id = id;

                var result = await tipoDeArticuloService.Update(tipo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de artículo: " + ex.Message);
            }
        }

        // DELETE <TiposArticuloController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese el id del tipo de artículo que desea eliminar.");
            try
            {
                var result = await tipoDeArticuloService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);
                else return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al eliminar el tipo de artículo: " + ex.Message);
            }

        }
    }
}
