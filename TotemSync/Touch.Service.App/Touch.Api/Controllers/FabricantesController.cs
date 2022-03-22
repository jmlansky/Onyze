using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Articulos;
using Framework.Helpers;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
   
    public class FabricantesController : BaseController
    {
        private readonly IFabricantesService fabricantesService;
        public FabricantesController(IConfiguration configuration, IFabricantesService fabricantesService): base(configuration)
        {
            this.fabricantesService = fabricantesService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var fabricantes = await fabricantesService.Get();
            var dto = new List<FabricanteDto>();
            if (fabricantes.Any())
            {
                foreach (var fabricante in fabricantes)
                    dto.Add(MapperEntidadDto.Mapper(fabricante, new FabricanteDto()));
            }
            return Ok(dto);
        }

        // GET <FabricantesController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var fabricante = await fabricantesService.Get(id);
            if (fabricante.Id.Equals(0))
                return NotFound();
            return Ok(MapperEntidadDto.Mapper(fabricante, new FabricanteDto()));
        }

        // GET <FabricantesController>/nombre/"Bago"
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre)
        {
            //var dto = new List<FabricanteDto>();
            var fabricantes = await fabricantesService.Get(nombre);
            return Ok(fabricantes);

            //var dto = new List<FabricanteDto>();
            //foreach (var item in fabricantes)
            //{
            //    var itemDto = MapperEntidadDto.Mapper(item, new FabricanteDto());
            //    dto.Add(itemDto);
            //}                

            //return Ok(dto);
        }

        // POST <FabricantesController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] FabricanteDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var fabricante = MapperEntidadDto.Mapper(dto, new Fabricante());
                var result = await fabricantesService.Insert(fabricante);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el fabricante de artículo: " + ex.Message);
            }
        }

        // PUT <FabricantesController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] FabricanteDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor seleccione un id válido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var fabricante = MapperEntidadDto.Mapper(dto, new Fabricante());
                fabricante.Id = id;

                var result = await fabricantesService.Update(fabricante);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el fabricante de artículo: " + ex.Message);
            }
        }

        // DELETE <FabricantesController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese el id del fabricante de artículo que desea eliminar.");
            try
            {
                var result = await fabricantesService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al eliminar el fabricante de artículo: " + ex.Message);
            }

        }
    }
}

