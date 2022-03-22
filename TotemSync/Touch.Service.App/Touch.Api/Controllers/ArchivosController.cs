using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Touch.Api.Dtos;
using Touch.Api.Dtos.Archivos;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Service.Archivos.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    [RequestSizeLimit(100000000)]
    public class ArchivosController : BaseController
    {
        private readonly IArchivosService archivosService;
        public ArchivosController(IConfiguration configuration, IArchivosService archivosService): base(configuration)
        {
            this.archivosService = archivosService;
        }

        // Delete <ArticulosController>/archivos
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var archivo = new Archivo()
            {
                Id = id,
                Modificado = DateTime.Now
            };

            var result = await archivosService.Delete(archivo);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            var archivo = await archivosService.Get(id);
            if (archivo == null || archivo.Id <= 0)
                return NotFound();

            var dto = MapperEntidadDto.Mapper(archivo, new ArchivoDto());
            dto.Miniaturas = archivo.Miniaturas.Select(x => MapperEntidadDto.Mapper(x, new ArchivoDto())).ToList();
            dto.Tipo = archivo.Tipo.Nombre;
            return Ok(dto);
        }

        [HttpPost("filtrados")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetFiltrados([FromBody] PostBuscarArchivosFiltradosDto filtros, int? pageNumber, int? pageSize)
        {
            if (filtros == null)
                return BadRequest("El filtro no debe ser nulo");
                        
            PagedResult result;
            if (filtros.FechaAltaFin == null &&
                filtros.FechaAltaInicio == null &&
                (filtros.IdTipoArchivo == null || filtros.IdTipoArchivo <= 0) &&
                string.IsNullOrWhiteSpace(filtros.Nombre) &&
                string.IsNullOrWhiteSpace(filtros.Size))
            {
                //obtener todos los archivos no eliminados
                result = archivosService.Get(pageNumber, pageSize).Result;
            }
            else 
            {
                var fitroArchivo = MapperEntidadDto.Mapper(filtros, new FiltroArchivos());
                result = archivosService.GetFiltrados(fitroArchivo, pageNumber, pageSize).Result;                
            }

            var response = new PagedResponse<ArchivoDto>();
            response.PageNumber = result.PageNumber;
            response.PageSize = result.PageSize;
            response.TotalPages = result.TotalPages;
            response.TotalRecords = result.TotalRecords;            
            response.List = MapearArchivoDto((PagedList<Archivo>)result.PagedList);

            return Ok(response);
        }
        
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromForm] PostArchivoDto dto)
        {
            var actionResult = ValidarInput(dto.File);
            if (!actionResult.GetType().Name.Equals("OkResult"))
                return actionResult;

            if (dto.File.FileName.Length > 200)
                return BadRequest("El nombre del archivo es demasiado largo, no debe superar los 195 caracteres");

            try
            {
                var archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;

                var result = await archivosService.UploadFile(archivo, dto.Small, dto.Medium, dto.Large);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromForm] PutArchivoDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            //var actionResult = ValidarInput(dto.File);
            //if (!actionResult.GetType().Name.Equals("OkResult"))
            //    return actionResult;

            if (dto.File !=null && dto.File.FileName.Length > 200)
                return BadRequest("El nombre del archivo es demasiado largo, no debe superar los 195 caracteres");

            try
            {
                var archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;
                archivo.Id = id;
               

                var result = await archivosService.UpdateFile(archivo);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("Articulos")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PostArchivoAlArticulo([FromForm] PostArchivoArticuloDto dto)
        {
            var actionResult = ValidarInput(dto.File);
            if (!actionResult.GetType().Name.Equals("OkResult"))
                return actionResult;

            try
            {
                dto.Descripcion ??= string.Empty;
                var archivo = new Archivo();

                archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;

                var result = await archivosService.UploadFileAsociarAlArticulo(dto.IdArticulo, archivo, dto.Small, dto.Medium, dto.Large);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("Gondolas")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PostArchivoAGondola([FromForm] PostArchivoGondolaDto dto)
        {
            var actionResult = ValidarInput(dto.File);
            if (!actionResult.GetType().Name.Equals("OkResult"))
                return actionResult;

            var referencias = new string[] { "Encabezado", "Fondo" };
            if (string.IsNullOrEmpty(dto.Referencia) || !referencias.Any(x => x.ToLower().Equals(dto.Referencia.ToLower())))
                return BadRequest("El archivo tiene que hacer referencia a un 'Encabezado' o 'Fondo'");

            try
            {
                dto.Descripcion ??= string.Empty;
                var archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;

                var result = await archivosService.UploadFileAsociarAGondola(dto.IdGondola, dto.Referencia, archivo, dto.Small);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("Categorias")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PostArchivoACategorias([FromForm] PostArchivoCategoriaDto dto)
        {
            var actionResult = ValidarInput(dto.File);

            if (!actionResult.GetType().Name.Equals("OkResult"))
                return actionResult;

            try
            {
                dto.Descripcion ??= string.Empty;

                var archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;

                var result = await archivosService.UploadFileAsociarACategoria(dto.IdCategoria, archivo, dto.Small, dto.Medium, dto.Large);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpPost("Publicaciones")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PostArchivoAPublicaciones([FromForm] PostArchivoPublicacionDto dto)
        {
            var actionResult = ValidarInput(dto.File);
            if (!actionResult.GetType().Name.Equals("OkResult"))
                return actionResult;

            try
            {
                dto.Descripcion ??= string.Empty;
                var archivo = MapperEntidadDto.Mapper(dto, new Archivo());
                archivo.File = dto.File;

                var result = await archivosService.UploadFileAsociarAPublicacion(dto.IdPublicacion, archivo, dto.Small, dto.Medium, dto.Large);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message.ToString());
            }
        }

        [HttpGet("Nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorNombre(string nombre, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(nombre))
                return BadRequest("Por favor ingrese un nombre válido");

            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            var result = await archivosService.Get(nombre);
            var pagedResponse = ObtenerDtoDeArchivosPaginado(pageNumber, pageSize, result);

            return Ok(pagedResponse);
        }

        [HttpGet("Tipo")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorTipo(long idTipo, int? pageNumber, int? pageSize)
        {
            if (idTipo <= 0)
                return BadRequest("Por favor ingrese un id válido");

            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            var result = await archivosService.GetPorTipo(idTipo);
            var pagedResponse = ObtenerDtoDeArchivosPaginado(pageNumber, pageSize, result);

            return Ok(pagedResponse);
        }

        [HttpGet("IdArchivoOriginal")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorIdOriginal(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await archivosService.GetPorIdOriginal(id);
            var dto = new List<ArchivoDto>();
            if (result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new ArchivoDto())).ToList();

            return Ok(dto);
        }

        [HttpGet("Size")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorSize(string size, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(size))
                return BadRequest("Por favor ingrese tamaño válido");

            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            var result = await archivosService.GetPorSize(size);

            var pagedResponse = ObtenerDtoDeArchivosPaginado(pageNumber, pageSize, result);
            return Ok(pagedResponse);
        }

        [HttpGet("SizeAndId")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorSizeAndId(long id, string size)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            if (string.IsNullOrEmpty(size))
                return BadRequest("Por favor ingrese tamaño válido");

            var result = await archivosService.GetPorSizeAndId(id, size);
            var dto = new List<ArchivoDto>();
            if (result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new ArchivoDto())).ToList();

            return Ok(dto);
        }

        [HttpGet("SizeAndTipo")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> SizeAndTipo(long idTipo, string size, int? pageNumber, int? pageSize)
        {
            if (idTipo <= 0)
                return BadRequest("Por favor ingrese un tipo válido");

            if (string.IsNullOrEmpty(size))
                return BadRequest("Por favor ingrese tamaño válido");

            var result = await archivosService.GetPorSizeAndTipo(idTipo, size);
            return Ok(ObtenerDtoDeArchivosPaginado(pageNumber, pageSize, result));
        }

        private IActionResult ValidarInput(IFormFile file)
        {
            if (file == null)
                return BadRequest("Por favor ingrese un archivo");

            var extensions = new string[] { ".jpg", ".png", ".mp4", ".JPG", ".jpeg" };
            if (!extensions.Any(x => x.ToLower().Equals(Path.GetExtension(file.FileName.ToLower()))))
                return BadRequest("Los formatos permitidos son jpg, png y mp4");

            return Ok();
        }

        private PagedResponse<ArchivoDto> ObtenerDtoDeArchivosPaginado(int? pageNumber, int? pageSize, IEnumerable<Archivo> result)
        {
            var pagedResponse = new PagedResponse<ArchivoDto>(pageNumber ?? 1, pageSize ?? 25, result.Count());

            if (result.Any())
            {
                var pagedListDeOriginales = new PagedList<Archivo>(result, pageNumber ?? 1, pageSize ?? 25);
                pagedResponse.List = MapearArchivoDto(pagedListDeOriginales);
            }

            return pagedResponse;
        }

        private List<ArchivoDto> MapearArchivoDto(PagedList<Archivo> pagedListDeOriginales)
        {
            var list = new List<ArchivoDto>();
            foreach (var archivoPaginado in pagedListDeOriginales)
            {
                var archivoDto = MapperEntidadDto.Mapper(archivoPaginado, new ArchivoDto());
                archivoDto.Miniaturas = archivoPaginado.Miniaturas.Select(x => MapperEntidadDto.Mapper(x, new ArchivoDto())).ToList();
                archivoDto.Tipo = archivoPaginado.Tipo.Nombre;
                list.Add(archivoDto);
            }

            return list;
        }      
    }
}