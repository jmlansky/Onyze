using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Atributos;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{    
    public class AtributosController : BaseController
    {
        private readonly IAtributosService atributosService;
        public AtributosController(IConfiguration configuration, IAtributosService atributosService): base(configuration)
        {
            this.atributosService = atributosService;
        }

        // GET: <AtributosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var atributos = await atributosService.Get();
            var dto = new List<GetAtributoResponse>();
            foreach (var atributo in atributos)
            {
                var itemDto = MapperEntidadDto.Mapper(atributo, new GetAtributoResponse());
                itemDto.Tipo = MapperEntidadDto.Mapper(atributo.TipoAtributo, new GetTipoAtributoDto());
                dto.Add(itemDto);
            } 
            
            return Ok(dto);
        }

        // GET <AtributosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            var atributo = await atributosService.Get(id);
            if (atributo.Id.Equals(0))
                return NotFound();

            var result = MapperEntidadDto.Mapper(atributo, new GetAtributoResponse());
            result.Tipo = MapperEntidadDto.Mapper(atributo.TipoAtributo, new GetTipoAtributoDto());

            return Ok(result);
        }


        // GET <AtributosController>/nombre/"medicamento"
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre)
        {
            var result = await atributosService.Get(nombre);
            var dto = new List<GetAtributoResponse>();

            foreach (var atributo in result)
            {
                var itemDto = MapperEntidadDto.Mapper(atributo, new GetAtributoResponse());
                itemDto.Tipo = MapperEntidadDto.Mapper(atributo.TipoAtributo, new GetTipoAtributoDto());
                dto.Add(itemDto);
            }

            return Ok(result);
        }

        // POST <AtributosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostAtributosDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var atributo = MapperEntidadDto.Mapper(dto, new Atributo());
                var result = await atributosService.Insert(atributo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el atributo: " + ex.Message);
            }
        }

        // PUT <AtributosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutAtributoDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor seleccione un id válido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("No puede haber nombre en blanco");

                var atributo = MapperEntidadDto.Mapper(dto, new Atributo());
                atributo.Id = id;

                var result = await atributosService.Update(atributo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al actualizar el atributo: " + ex.Message);
            }
        }

        // DELETE <AtributosController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese el id del tipo de artículo que desea eliminar.");
            try
            {
                var result = await atributosService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);
                else return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al eliminar el atributo: " + ex.Message);
            }
        }
    }
}
