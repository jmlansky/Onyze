using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.TiposAtributo;
using Touch.Core.Articulos;
using Touch.Service.Articulos;


namespace Touch.Api.Controllers
{
    public class TiposDeAtributoController : BaseController
    {
        private readonly ITiposDeAtributoService tiposDeAtributoService;
        private readonly IAtributosService atributosService;
        public TiposDeAtributoController(IConfiguration configuration, ITiposDeAtributoService tiposDeAtributoService,
            IAtributosService atributosService): base(configuration)
        {
            this.tiposDeAtributoService = tiposDeAtributoService;
            this.atributosService = atributosService;
        }

        // GET: <TiposDeAtributoController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var tipos = await tiposDeAtributoService.Get();

            var dto = new List<TipoAtributoDto>();
            if (!tipos.Any())
                return Ok(dto);

            dto = tipos.Select(x => MapperEntidadDto.Mapper(x, new TipoAtributoDto())).ToList();

            foreach (var tipo in dto)
            {
                var atributos = await atributosService.GetAtributosDelTipo(tipo.Id);
                tipo.Atributos = atributos.Select(x => MapperEntidadDto.Mapper(x, new AtributoDelTipoDto())).ToList();
            }

            return Ok(dto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int id)
        {
            var tipo = await tiposDeAtributoService.Get(id);
            if (tipo.Id.Equals(0))
                return NotFound();
                        
            var tipoDto = MapperEntidadDto.Mapper(tipo, new TipoAtributoDto());
            
            var atributos = await atributosService.GetAtributosDelTipo(tipo.Id);
            tipoDto.Atributos = atributos.Select(x => MapperEntidadDto.Mapper(x, new AtributoDelTipoDto())).ToList();

            return Ok(tipoDto);
        }

        // GET <TiposArticuloController>/nombre/"medicamento"
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre)
        {
            var tipos = await tiposDeAtributoService.Get(nombre);
            var dto = new List<TipoAtributoDto>();
            if (!tipos.Any())
                return Ok(dto);

            dto = tipos.Select(x => MapperEntidadDto.Mapper(x, new TipoAtributoDto())).ToList();

            foreach (var tipo in dto)
            {
                var atributos = await atributosService.GetAtributosDelTipo(tipo.Id);
                tipo.Atributos = atributos.Select(x => MapperEntidadDto.Mapper(x, new AtributoDelTipoDto())).ToList();
            }
            return Ok(dto);
        }

        // POST <TiposDeAtributoController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostTipoAtributoDto dto)
        {
            try
            {
                var tipo = MapperEntidadDto.Mapper(dto, new TipoAtributo());

                var result = await tiposDeAtributoService.Insert(tipo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de atributo: " + ex.Message);
            }
        }

        // PUT <TiposDeAtributoController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, [FromBody] TipoAtributoDto dto)
        {
            try
            {
                var tipo = MapperEntidadDto.Mapper(dto, new TipoAtributo());
                tipo.Id = id;

                var result = await tiposDeAtributoService.Update(tipo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de atributo: " + ex.Message);
            }
        }

        // DELETE <TiposDeAtributoController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var tipo = new TipoAtributo() { Id = id };

                var result = await tiposDeAtributoService.Delete(tipo);
                if (!result.HasErrors)
                    return Ok(result);
                else
                    return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar el tipo de atributo: " + ex.Message);
            }
        }
    }
}
