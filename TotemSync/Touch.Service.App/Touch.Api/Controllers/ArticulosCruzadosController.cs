using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.ArticulosMultiples;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{    
    public class ArticulosCruzadosController : BaseController
    {
        private readonly IArticulosService articulosService;
        public ArticulosCruzadosController(IConfiguration configuration, IArticulosService articulosService): base(configuration)
        {
            this.articulosService = articulosService;
        }

        // POST <AriculosCruzadosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] IEnumerable<PostArticulosMultiplesDto> articulosDto)
        {
            try
            {
                var articulos = new List<ArticuloMultiple>();
                foreach (var articuloDto in articulosDto)
                {
                    if (articuloDto.IdOrigen.Equals(articuloDto.IdDestino))
                        return BadRequest("No se puede insertar el mismo producto como cruzado.");

                    var articulo = MapperEntidadDto.Mapper(articuloDto, new ArticuloMultiple());
                    articulos.Add(articulo);
                }

                var result = await articulosService.InsertarCruzados(articulos);
                if (!result.HasErrors)
                    return Ok(result);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE <AriculosCruzadosController>/5
        [HttpDelete]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete([FromBody] DeleteArticulosMultiplesDto dto)
        {
            try
            {
                var articulo = MapperEntidadDto.Mapper(dto, new ArticuloMultiple());
                var result = await articulosService.DeleteCruzado(articulo);
                if (!result.HasErrors)
                    return Ok(result);

                return  StatusCode((int)result.StatusCode, result); 
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
