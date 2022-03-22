using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Archivos;
using Touch.Api.Dtos.CategoriasDeArticulo;
using Framework.Helpers;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    public class CategoriasArticuloController : BaseController
    {
        private readonly ICategoriasDeArticulosService categoriasDeArticulosService;        
        public CategoriasArticuloController(IConfiguration configuration, ICategoriasDeArticulosService categoriasDeArticulosService): base(configuration)
        {
            this.categoriasDeArticulosService = categoriasDeArticulosService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var categorias = await categoriasDeArticulosService.Get();
            var dto = new List<CategoriaDto>();
            foreach (var cat in categorias)
                dto.Add(MapearSubcategorias(cat));

            return Ok(dto);
        }        

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            var categoria = await categoriasDeArticulosService.Get(id);

            if (categoria.Id.Equals(0))
                return NotFound();

            var dto = MapearSubcategorias(categoria);

            return Ok(dto);
        }

        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre)
        {
            var categorias = await categoriasDeArticulosService.Get(nombre);
            var dto = new List<CategoriaDto>();
            foreach (var cat in categorias)
                dto.Add(MapearSubcategorias(cat));

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostCategoriaDto dto)
        {
            try
            {
                var categoria = MapperEntidadDto.Mapper(dto, new CategoriaDeArticulo());                            

                var result = await categoriasDeArticulosService.Insert(categoria);
                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de artículo: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutCategoriaDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor seleccione un id válido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");


                var categoria = MapperEntidadDto.Mapper(dto, new CategoriaDeArticulo());
                categoria.Id = id;
                

                var result = await categoriasDeArticulosService.Update(categoria);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese el id del tipo de artículo que desea eliminar.");
            try
            {
                var result = await categoriasDeArticulosService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);
                else return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al eliminar el tipo de artículo: "+ ex.Message);
            }
        }

        private CategoriaDto MapearSubcategorias(CategoriaDeArticulo categoria)
        {
            var dto = MapperEntidadDto.Mapper(categoria, new CategoriaDto());
            if (categoria.Archivo != null && categoria.Archivo.Id > 0)
                dto.Archivo = MapperEntidadDto.Mapper(categoria.Archivo, new ArchivoDto());

            foreach (var subcategoria in categoria.Subcategorias)
                dto.Subcategorias.Add(MapearSubcategorias(subcategoria));

            return dto;
        }
    }
}
