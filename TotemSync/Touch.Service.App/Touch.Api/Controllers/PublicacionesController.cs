using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Archivos;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.Gondolas;
using Touch.Api.Dtos.Publicaciones;
using Touch.Api.Dtos.TiposObjetoPublicitar;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Core.Publicaciones;
using Touch.Service.Publicaciones;
using Touch.Api.Dtos;
using PagedList;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
     public class PublicacionesController : BaseController
    {
        private readonly IPublicacionesService publicacionService;
        public PublicacionesController(IConfiguration configuration, IPublicacionesService publicacionService): base(configuration)
        {
            this.publicacionService = publicacionService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await publicacionService.Get(pageNumber, pageSize);
            var response = new PagedResponse<PublicacionDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Publicacion>)result.PagedList).Select(x => MapearPublicacionADto(x)).ToList();           

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await publicacionService.Get(id);

            if (result == null || result.Id <= 0)
                return NotFound();

            return Ok(MapearPublicacionADto(result));
        }

        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre, int? pageNumber, int? pageSize)
        {
            var result = await publicacionService.Get(nombre, pageNumber, pageSize);
            var response = new PagedResponse<PublicacionDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Publicacion>)result.PagedList).Select(x => MapearPublicacionADto(x)).ToList();

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostPublicacionDto publicacionDto)
        {
            if (publicacionDto.ObjetosAPublicitar != null && !publicacionDto.ObjetosAPublicitar.Any())
                return BadRequest("Por favor ingrese lo que desea publicitar");

            var publicacion = MapperEntidadDto.Mapper(publicacionDto, new Publicacion());

            if (publicacionDto.ObjetosAPublicitar != null && publicacionDto.ObjetosAPublicitar.Any())
                publicacion.ObjetosAPublicitar = publicacionDto.ObjetosAPublicitar.Select(x => MapperEntidadDto.Mapper(x, new ObjetoAPublicar())).ToList();

            var result = await publicacionService.Insert(publicacion);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutPublicacionDto publicacionDto)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var publicacion = MapperEntidadDto.Mapper(publicacionDto, new Publicacion());
            publicacion.Id = id;

            var result = await publicacionService.Update(publicacion);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpPost("{id}/Objetos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post(long id, [FromBody] List<PostObjetoPublicitadoDto> dto)
        {
            if (!dto.Any())
                return BadRequest("Por favor ingrese algun objeto");

            var objetos = new List<ObjetoAPublicar>();
            foreach (var item in dto)
                objetos.Add(new ObjetoAPublicar() { IdObjeto = item.IdObjeto, IdPantalla = id, IdTipo = item.IdTipo });

            var result = new ServiceResult();
            foreach (var item in objetos)
            {
                item.IdPantalla = id;
                result = await publicacionService.InsertObjeto(item);
            }

            return StatusCode((int)result.StatusCode, result);
        }


        [HttpDelete("{id}/Objetos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id, [FromBody] List<DeleteObjetoDto> dto)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var objetos = new List<ObjetoAPublicar>();
            foreach (var item in dto)
                objetos.Add(new ObjetoAPublicar() { IdObjeto = item.IdObjeto, IdPantalla = id, IdTipo = item.IdTipo });

            var result = await publicacionService.DeleteObjectsFromPantalla(objetos);
            return StatusCode((int)result.StatusCode, result);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await publicacionService.Delete(new Publicacion() { Id = id });
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        private static PublicacionDto MapearPublicacionADto(Publicacion item)
        {
            var dtoItem = MapperEntidadDto.Mapper(item, new PublicacionDto());
            if (item.Archivo != null && item.Archivo.Id > 0)
            {
                dtoItem.Archivo = MapperEntidadDto.Mapper(item.Archivo, new ArchivoDto());
                dtoItem.Archivo.Miniaturas = item.Archivo.Miniaturas.Select(x => MapperEntidadDto.Mapper(x, new ArchivoDto())).ToList();
            }

            foreach (var objeto in item.ObjetosAPublicitar)
            {
                var objetoDto = new ObjetoPublicitadoDto();
                if (objeto.Objeto.GetType().Name == "Articulo")
                    objetoDto.Objeto = MapperEntidadDto.Mapper(objeto.Objeto, new ArticuloDto());

                if (objeto.Objeto.GetType().Name == "Gondola")
                    objetoDto.Objeto = MapperEntidadDto.Mapper(objeto.Objeto, new GondolaDto());

                objetoDto.Tipo = MapperEntidadDto.Mapper(objeto.Tipo, new TipoObjetoPublicitarDto());

                dtoItem.ObjetosPublicitados.Add(objetoDto);
            }
            return dtoItem;
        }
    }
}
