using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Estantes;
using Framework.Helpers;
using Touch.Core.Gondolas;
using Touch.Service.Gondolas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Archivos;
using Touch.Core.Archivos;

namespace Touch.Api.Controllers
{

    public class EstantesController : BaseController
    {
        private readonly IEstantesService estantesService;
        public EstantesController(IConfiguration configuration, IEstantesService estantesService) : base(configuration)
        {
            this.estantesService = estantesService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var estantes = (await estantesService.Get()).ToList();
            var dto = new List<EstanteDto>();
            if (estantes != null && estantes.Any())
            {
                foreach (var estante in estantes)
                {
                    var estanteDto = MapperEntidadDto.Mapper(estante, new EstanteDto());
                    MapearDecoraciones(estante.Decoraciones, estanteDto);
                    MapearArticulosEstante(estante.Articulos, estanteDto);

                    dto.Add(estanteDto);
                }
            }

            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            try
            {
                var estante = await estantesService.Get(id);
                if (estante == null || estante.Id <= 0)
                    return NotFound();

                var dto = MapperEntidadDto.Mapper(estante, new EstanteDto());
                MapearDecoraciones(estante.Decoraciones, dto);
                MapearArticulosEstante(estante.Articulos, dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostEstanteDto dto)
        {
            var estante = MapperEntidadDto.Mapper(dto, new Estante());
            MapearDecoraciones(dto.Decoraciones, estante);
            MapearArticulosEstante(dto.Articulos, estante);

            var result = await estantesService.Insert(estante);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutEstanteDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var estante = MapperEntidadDto.Mapper(dto, new Estante());
            estante.Id = id;

            MapearDecoraciones(dto.Decoraciones, estante);
            MapearArticulosEstante(dto.Articulos, estante);

            var result = await estantesService.Update(estante);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var estante = new Estante() { Id = id };
            var result = await estantesService.Delete(estante);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPost("{id}/Articulos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post(long id, [FromBody] List<PostArticuloEstanteDto> dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id valido");

            if (!dto.Any())
                return BadRequest("Por favor articulos a insertar");

            var articulosPorEstante = new List<ArticuloEstante>();
            foreach (var articuloDto in dto)
            {
                var articuloPorEstante = new ArticuloEstante();
                articuloPorEstante = MapperEntidadDto.Mapper(articuloDto, articuloPorEstante);
                articuloPorEstante.Decoraciones = MapperEntidadDto.Mapper(articuloDto.Decoraciones, new List<ArticuloDecoracion>());
                //if (articuloDto.Decoracion.Destacado != null /*&& articuloDto.Decoracion.Destacado.IdArticuloDecoracion > 0*/)
                //    articuloPorEstante.Decoracion.Destacado = MapperEntidadDto.Mapper(articuloDto.Decoracion.Destacado, new ArticuloDestacado());
                articuloPorEstante.IdEstante = id;
                articulosPorEstante.Add(articuloPorEstante);
            }
            var result = await estantesService.AsociarArticulosAlEstante(articulosPorEstante);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}/Articulos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutArticuloEstanteDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id valido");

            if (!dto.Articulos.Any())
                return BadRequest("Por favor articulos a insertar");

            var articulosPorEstante = new List<ArticuloEstante>();
            foreach (var articuloDto in dto.Articulos)
            {
                var articuloPorEstante = new ArticuloEstante();
                articuloPorEstante = MapperEntidadDto.Mapper(articuloDto, articuloPorEstante);
                articuloPorEstante.IdEstante = id;
                articuloPorEstante.Decoraciones = MapperEntidadDto.Mapper(articuloDto.Decoraciones, new List<ArticuloDecoracion>());
                articulosPorEstante.Add(articuloPorEstante);
            }
            var result = await estantesService.ActualizarArticulosDelEstante(articulosPorEstante);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}/Articulos/all")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> DeleteTodosLosArticulosDelEstante(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id valido");

            var result = await estantesService.EliminarArticulosDelEstante(id);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}/Articulos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> DeleteArticulosDelEstante(long id, [FromBody] DeleteArticuloEstanteDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id valido");

            dto.IdsArticulos.RemoveAll(x => x == 0);
            if (!dto.IdsArticulos.Any())
                return BadRequest("Por favor articulos a eliminar");

            var listaDeArticulos = new List<ArticuloEstante>();
            foreach (var articuloDto in dto.IdsArticulos)
                listaDeArticulos.Add(new ArticuloEstante() { IdEstante = id, IdArticulo = articuloDto });

            var result = await estantesService.EliminarArticulosDelEstante(listaDeArticulos);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id}/Articulos")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetArticulosDelEstante(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id valido");

            var articulos = await estantesService.GetArticulosDelEstante(id);
            var estanteDto = new EstanteDto();
            MapearArticulosEstante(articulos.ToList(), estanteDto);

            return Ok(estanteDto.Articulos);
        }


        private void MapearDecoraciones(List<PostDecoracionEstanteDto> decoracionesDto, Estante estante)
        {
            foreach (var decoDto in decoracionesDto)
            {
                var deco = MapperEntidadDto.Mapper(decoDto, new EstanteDecoracion());
                deco.IdEstante = estante.Id;

                estante.Decoraciones.Add(deco);
            }
        }
        private void MapearDecoraciones(List<EstanteDecoracion> decoraciones, EstanteDto estanteDto)
        {
            foreach (var deco in decoraciones)
            {
                var decoDto = MapperEntidadDto.Mapper(deco, new DecoracionEstanteDto());

                estanteDto.Decoraciones.Add(decoDto);
            }
        }
        private void MapearArticulosEstante(List<PostArticuloEstanteDto> articulosDto, Estante estante)
        {
            foreach (var articuloDto in articulosDto)
            {
                var articulo = MapperEntidadDto.Mapper(articuloDto, new ArticuloEstante());
                articulo.Decoraciones = MapperEntidadDto.Mapper(articuloDto.Decoraciones, new List<ArticuloDecoracion>());
                //articulo.Decoracion.Destacado = MapperEntidadDto.Mapper(articuloDto.Decoracion.Destacado, new ArticuloDestacado());
                estante.Articulos.Add(articulo);
            }
        }
        private void MapearArticulosEstante(List<ArticuloEstante> articulos, EstanteDto estanteDto)
        {
            foreach (var articulo in articulos)
            {
                var articuloDto = MapperEntidadDto.Mapper(articulo, new ArticuloEstanteDto());

                if (articulo.Decoraciones != null)
                {
                    articuloDto.Decoraciones = MapperEntidadDto.Mapper(articulo.Decoraciones, new List<ArticuloDecoracionDto>());
                    //if (articulo.Decoracion.Destacado != null)
                    //    articuloDto.Decoracion.Destacado = MapperEntidadDto.Mapper(articulo.Decoracion.Destacado, new ArticuloDestacadoDto());
                }

                estanteDto.Articulos.Add(articuloDto);
            }
        }
    }
}
