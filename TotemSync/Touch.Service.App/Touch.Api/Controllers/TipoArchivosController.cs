using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.TipoArchivo;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
    public class TipoArchivosController : BaseController
    {
        private readonly ISingleEntityComunService<TipoArchivo> tipoArchivosService;
        public TipoArchivosController(IConfiguration configuration, ISingleEntityComunService<TipoArchivo> tipoArchivosService): base(configuration)
        {
            this.tipoArchivosService = tipoArchivosService;
        }

        // GET: <tipoArchivosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await tipoArchivosService.Get();
            var dto = new List<TipoArchivoDto>();
            if (result.Any())
            {
                foreach (var tipoArchivo in result)
                    dto.Add(MapperEntidadDto.Mapper(tipoArchivo, new TipoArchivoDto()));
            }

            return Ok(dto);
        }

        // GET <tipoArchivosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var result = await tipoArchivosService.Get(id);
            if (result != null && result.Id > 0)
                return Ok(MapperEntidadDto.Mapper(result, new TipoArchivoDto()));
            return NotFound();
        }

        // GET <tipoArchivosController>/nombre
        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre)
        {
            var result = await tipoArchivosService.Get(nombre);
            var dto = new List<TipoArchivoDto>();
            if (result.Any())
            {
                foreach (var tipoArchivo in result)
                    dto.Add(MapperEntidadDto.Mapper(tipoArchivo, new TipoArchivoDto()));
            }

            return Ok(dto);
        }


        // POST <tipoArchivosController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostTipoArchivoDto dto)
        {
            var tipoArchivo = MapperEntidadDto.Mapper(dto, new TipoArchivo());
            tipoArchivo.Creado = DateTime.Now;

            var result = await tipoArchivosService.Insert(tipoArchivo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // PUT <tipoArchivosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, PutTipoArchivoDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var tipoArchivo = MapperEntidadDto.Mapper(dto, new TipoArchivo());
            tipoArchivo.Id = id;
            tipoArchivo.Modificado = DateTime.Now;

            var result = await tipoArchivosService.Update(tipoArchivo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }

        // DELETE <tipoArchivosController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var tipoArchivo = new TipoArchivo()
            {
                Id = id,
                Modificado = DateTime.Now,
            };

            var result = await tipoArchivosService.Delete(tipoArchivo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
