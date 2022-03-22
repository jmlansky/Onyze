using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.Atributos;
using Touch.Service.Articulos;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{    
    public class AtributosDelArticuloController : BaseController
    {
        private readonly IArticulosService articulosService;
        public AtributosDelArticuloController(IConfiguration configuration, IArticulosService articulosService): base(configuration)
        {
            this.articulosService = articulosService;
        }        

        [HttpPost("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post(long id, [FromBody] PostAtributosDelArticuloDto atributos)
        {
            if (id <= 0)
                return BadRequest("El id del artículo tiene que ser mayor a 0");

            if (!atributos.IdsAtributos.Any())
                return BadRequest("Por favor seleccione atributos a asociar.");

            var anyDuplicate = atributos.IdsAtributos.GroupBy(x => x).Any(g => g.Count() > 1);
            if (anyDuplicate)
                return BadRequest("Tiene valores repetidos.");

            var result = await articulosService.AsociarAtributosAlArticulo(id, atributos.IdsAtributos);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id, [FromBody] PostAtributosDelArticuloDto atributos)
        {
            if (id <= 0)
                return BadRequest("El id del artículo tiene que ser mayor a 0");

            if (!atributos.IdsAtributos.Any())
                return BadRequest("Por favor seleccione atributos a asociar.");

            var anyDuplicate = atributos.IdsAtributos.GroupBy(x => x).Any(g => g.Count() > 1);
            if (anyDuplicate)
                return BadRequest("Tiene valores repetidos.");

            var result = await articulosService.DeleteAtributosDelArticulo(id, atributos.IdsAtributos);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("El id del artículo tiene que ser mayor a 0");

                var dto = new List<GetAtributoResponse>();
                foreach (var result in await articulosService.GetAtributosAlArticulo(id))
                    dto.Add(MapperEntidadDto.Mapper(result, new GetAtributoResponse()));

                return Ok(dto);
            }
            catch (Exception)
            {
                return BadRequest("Hubo un error al obtener los atributos al articulo");
            }
        }
        
    }
}
