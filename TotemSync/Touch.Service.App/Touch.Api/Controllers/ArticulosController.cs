using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PagedList;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.CategoriasDeArticulo;
using Touch.Api.Dtos.CodigosDeBarra;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Framework.Comun.Dtos.Responses;
using Touch.Api.Dtos.Archivos;
using Touch.Core.Archivos;
using Touch.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Controllers
{
    public class ArticulosController : BaseController
    {
        private readonly IArticulosService articulosService;
        private readonly ILogger<ArticulosController> logger;
        public ArticulosController(IConfiguration configuration, ILogger<ArticulosController> logger, IArticulosService articulosService) : base(configuration)
        {
            this.articulosService = articulosService;
            this.logger = logger;
        }

        #region Articulos


        // GET: <ArticulosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            try
            {
                var articulos = await articulosService.GetArticulosPaginados(pageNumber ?? 1, pageSize ?? 25);

                var listaDto = MapearListaDeArticulosAListaDeDtos(articulos.Articulos);
                foreach (var articuloDto in listaDto)
                {
                    var t = Task.Run(() =>
                    {
                        articuloDto.IdsAlternativos = articulosService.GetArticulosAlternativos(articuloDto.Id, false).Result.Select(x => x.Id).ToList();
                        articuloDto.IdsCruzados = articulosService.GetArticulosCruzados(articuloDto.Id, false).Result.Select(x => x.Id).ToList();
                    });
                }

                var dto = new PagedResponse<ArticuloDto>(pageNumber ?? 1, pageSize ?? 25, articulos.TotalRecords);
                dto.List.AddRange(listaDto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/Simple")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetSimple(int? pageNumber, int? pageSize)
        {

            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            try
            {
                var articulos = await articulosService.GetArticulosPaginados(pageNumber ?? 1, pageSize ?? 25, false);

                var dto = new PagedResponse<GetArticulosSimpleResponse>(pageNumber ?? 1, pageSize ?? 25, articulos.TotalRecords);
                MapearArticulosSimplesDto(articulos, dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetWithDeleted")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetWithDeleted(int? pageNumber, int? pageSize, string fechaSincro)
        {

            var result = await articulosService.GetWithDeleted(pageNumber, pageSize, fechaSincro);


            var current = await articulosService.GetCurentCount();

            var response = new PagedResponse<ArticuloDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords               ,
                CurrentRecords = current

            };

            response.List = MappearArticulosaADto((PagedList<Articulo>)result.PagedList);

            return Ok(response);
        }

        [HttpGet("/Simple/Filtrados")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetSimple(int? pageNumber, int? pageSize, string filtro)
        {
            if (string.IsNullOrEmpty(filtro))
                return BadRequest("El filtro no puede estar vacío");

            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            try
            {
                var articulos = await articulosService.GetArticulosPaginadosFiltrados(pageNumber ?? 1, pageSize ?? 25, filtro);

                var dto = new PagedResponse<GetArticulosSimpleResponse>(pageNumber ?? 1, pageSize ?? 25, articulos.TotalRecords);
                MapearArticulosSimplesDto(articulos, dto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return BadRequest(ex.Message);
            }
        }

        // GET <ArticulosController>/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            var articulo = await articulosService.GetArticulo(id);

            if (articulo == null || articulo.Id <= 0)
                return NotFound();

            if (articulo.Id.Equals(0))
                return NotFound();
            var articuloDto = MapearArticuloADto(articulo);

            var t = Task.Run(() =>
            {
                articuloDto.IdsAlternativos = articulosService.GetArticulosAlternativos(articulo.Id, false).Result.Select(x => x.Id).ToList();
                articuloDto.IdsCruzados = articulosService.GetArticulosCruzados(articulo.Id, false).Result.Select(x => x.Id).ToList();
            });

            t.Wait();

            return Ok(articuloDto);
        }

        // GET <ArticulosController>/5
        [HttpGet("{id}/Simple")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetSimple(long id)
        {
            var articulo = await articulosService.GetArticulo(id, false);
            if (articulo.Id.Equals(0))
                return NotFound();

            return Ok(MapperEntidadDto.Mapper(articulo, new GetArticulosSimpleResponse()));
        }

        // POST <ArticulosController>/filtrados
        [HttpPost("filtrados")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetFiltrados([FromBody] PostBuscarArticulosFiltradosDto filtros, int? pageNumber, int? pageSize)
        {

            var articuloABuscar = MapearDtoAArticulo(filtros);
            var resultadosDeBusqueda = await articulosService.GetArticulos(articuloABuscar);

            //Mappear Articulo
            var dto = MapearListaDeArticulosAListaDeDtos(resultadosDeBusqueda);

            foreach (var articuloDto in dto)
            {
                var t = Task.Run(() =>
                {
                    articuloDto.IdsAlternativos = articulosService.GetArticulosAlternativos(articuloDto.Id, false).Result.Select(x => x.Id).ToList();
                    articuloDto.IdsCruzados = articulosService.GetArticulosCruzados(articuloDto.Id, false).Result.Select(x => x.Id).ToList();
                });

                t.Wait();
                //return Ok(articuloDto);
            }

            var pagedList = new PagedList<ArticuloDto>(dto, pageNumber ?? 1, pageSize ?? 25);
            var response = new PagedResponse<ArticuloDto>(pageNumber ?? 1, pageSize ?? 25, dto.Count())
            {
                List = pagedList.Select(x => x).ToList()
            };
            return Ok(response);
        }

        // GET <ArticulosController>/archivos
        [HttpGet("{id}/archivos")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetArchivos(long id)
        {
            IEnumerable<Archivo> resultadosDeBusqueda = await articulosService.GetArchivosDelArticulo(id);

            //Mappear lista de archivos
            var dto = new List<ArchivoDto>();
            foreach (var archivo in resultadosDeBusqueda)
            {
                var itemDto = MapperEntidadDto.Mapper(archivo, new ArchivoDto());
                itemDto.Tipo = archivo.Tipo != null ? archivo.Tipo.Nombre : string.Empty;
                dto.Add(itemDto);
            }

            return Ok(dto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id valido.");

                var result = await articulosService.Delete(id);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception)
            {
                return BadRequest("Hubo un error al eliminar su artículo.");
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostArticuloDto articuloDto)
        {
            if (articuloDto.CodigosDeBarra != null && articuloDto.CodigosDeBarra.Any(x => string.IsNullOrEmpty(x)))
                return BadRequest("No deben haber codigos de barra en vacíos.");

            try
            {
                var articulo = MapperEntidadDto.Mapper(articuloDto, new Articulo());

                articulo.Categorias = articuloDto.Categorias.Select(x => new CategoriaDeArticulo() { Id = x }).ToList();

                foreach (var codigo in articuloDto.CodigosDeBarra)
                    articulo.Codigos.Add(new CodigoDeBarras() { EAN = codigo, Creado = DateTime.Now });

                if (articuloDto.Etiquetas != null && articuloDto.Etiquetas.Any())
                    articulo.Etiquetas = string.Join(",", articuloDto.Etiquetas);

                if (articuloDto.Atributos != null && articuloDto.Atributos.Any())
                    articulo.Atributos = articuloDto.Atributos.Select(x => new Atributo() { Id = x }).ToList();

                var result = await articulosService.Insert(articulo);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un problema al insertar el artículo: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutArticuloDto articuloDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido");

                if (string.IsNullOrWhiteSpace(articuloDto.Nombre) || string.IsNullOrWhiteSpace(articuloDto.SKU))
                    return BadRequest("Por favor ingrese nombre y sku");

                var articulo = MapperEntidadDto.Mapper(articuloDto, new Articulo());
                articulo.Id = id;

                if (articuloDto.Etiquetas != null && articuloDto.Etiquetas.Any())
                    articulo.Etiquetas = string.Join(",", articuloDto.Etiquetas);

                if (articuloDto.Categorias != null && articuloDto.Categorias.Any())
                    articulo.Categorias = articuloDto.Categorias.Select(x => new CategoriaDeArticulo() { Id = x }).ToList();

                if (articuloDto.Atributos != null && articuloDto.Atributos.Any())
                    articulo.Atributos = articuloDto.Atributos.Select(x => new Atributo() { Id = x }).ToList();

                if (articuloDto.CodigosDeBarra != null && articuloDto.CodigosDeBarra.Any())
                    articulo.Codigos = articuloDto.CodigosDeBarra.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => new CodigoDeBarras() { EAN = x, Creado = DateTime.Now }).ToList();

                var result = await articulosService.Update(articulo);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un problema al insertar el artículo: " + ex.Message);
            }

        }

        #endregion

        #region Articulos multiples

        [HttpGet("{id}/alternativos")]
        [Authorize(Roles = "Admin, Super, Guest")]

        public async Task<IActionResult> GetAlternativos(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var articulos = await articulosService.GetArticulosAlternativos(id, false);
            var articulosDto = new List<GetArticulosSimpleResponse>();
            foreach (var articulo in articulos.Where(x => x.Id > 0))
                articulosDto.Add(MapperEntidadDto.Mapper(articulo, new GetArticulosSimpleResponse()));

            return Ok(articulosDto);
        }

        [HttpGet("relaciones")]
        [Authorize(Roles = "Admin, Super, Guest")]

        public async Task<IActionResult> GetRelaciones(int? pageNumber, int? pageSize, string fechaSincro)
        {
            var result = await articulosService.GetRelaciones(pageNumber, pageSize, fechaSincro);


            var response = new PagedResponse<RelacionDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = MappeaRelacionesDto((PagedList<Relacion>)result.PagedList);

            return Ok(response);


        }



        private List<RelacionDto> MappeaRelacionesDto(PagedList<Relacion> pagedList)
        {
            var list = new List<RelacionDto>();
            foreach (var relacion in pagedList)
            {
                var relacionDto = MapperEntidadDto.Mapper(relacion, new RelacionDto());


                list.Add(relacionDto);
            }

            return list;
        }


        [HttpGet("{id}/cruzados")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetCruzados(long id)
        {
            var articulos = await articulosService.GetArticulosCruzados(id, false);
            var articulosDto = new List<GetArticulosSimpleResponse>();
            foreach (var articulo in articulos.Where(x => x.Id > 0))
                articulosDto.Add(MapperEntidadDto.Mapper(articulo, new GetArticulosSimpleResponse()));

            return Ok(articulosDto);
        }

        #endregion

        #region Metodos privados
        private List<ArticuloDto> MapearListaDeArticulosAListaDeDtos(IEnumerable<Articulo> articulos)
        {
            var articulosDto = new List<ArticuloDto>();
            foreach (var articulo in articulos)
            {
                var dto = MapearArticuloADto(articulo);
                articulosDto.Add(dto);
            }

            return articulosDto;
        }

        private ArticuloDto MapearArticuloADto(Articulo articulo)
        {
            var dto = MapperEntidadDto.Mapper(articulo, new ArticuloDto());
            //mapear etiquetas
            if (!string.IsNullOrEmpty(articulo.Etiquetas))
                dto.Etiquetas = articulo.Etiquetas.Split(',').Select(x => x.Trim()).ToList();

            //Mappear fabricante
            if (articulo.Fabricante != null && articulo.IdFabricante > 0)
                dto.Fabricante = MapperEntidadDto.Mapper(articulo.Fabricante, new FabricanteDto());

            //Mappear atributos 
            MapearAtributosDelArticuloADto(articulo, dto);

            //Mapear categoria de articulo
            dto.Categorias = articulo.Categorias.Select(x => MapperEntidadDto.Mapper(x, new CategoriaDto())).ToList();

            //Mapear tipo de articulo
            if (articulo.Tipo.Id > 0)
                dto.TipoArticulo = MapperEntidadDto.Mapper(articulo.Tipo, new TipoArticuloDto());

            //Mapear archivos
            if (articulo.Archivos.Any())
            {
                dto.Archivos = new List<ArchivoDto>();
                foreach (var archivo in articulo.Archivos)
                {
                    var item = MapperEntidadDto.Mapper(archivo, new ArchivoDto());
                    item.Tipo = archivo.Tipo.Nombre;
                    dto.Archivos.Add(item);
                }
            }

            //mapear codigos de barra
            if (articulo.Codigos.Any())
                dto.CodigosDeBarra = articulo.Codigos.Select(x => MapperEntidadDto.Mapper(x, new CodigoDeBarrasDto()));

            return dto;
        }

        private List<ArticuloDto> MappearArticulosaADto(PagedList<Articulo> pagedList)
        {
            var list = new List<ArticuloDto>();
            foreach (var articulo in pagedList)
            {

                var dto = MapearArticuloADto(articulo);


                list.Add(dto);
            }

            return list;
        }

        private void MapearAtributosDelArticuloADto(Articulo articulo, ArticuloDto dto)
        {
            foreach (var atributo in articulo.Atributos)
            {
                var atributoDto = MapperEntidadDto.Mapper(atributo, new AtributoDto());
                atributoDto.TipoAtributo = MapperEntidadDto.Mapper(atributo.TipoAtributo, new TipoAtributoDto());
                dto.Atributos.Add(atributoDto);
            }
        }

        private Articulo MapearDtoAArticulo(PostBuscarArticulosFiltradosDto filtros)
        {
            var articulo = new Articulo
            {
                Fabricante = (filtros.IdFabricante > 0 || !string.IsNullOrWhiteSpace(filtros.NombreFabricante))
                    ? new Fabricante() { Id = filtros.IdFabricante, Nombre = filtros.NombreFabricante }
                    : new Fabricante(),

                Tipo = (filtros.IdTipoArticulo > 0 || !string.IsNullOrWhiteSpace(filtros.TipoArticulo))
                    ? new TipoArticulo() { Id = filtros.IdTipoArticulo, Nombre = filtros.TipoArticulo }
                    : new TipoArticulo(),
            };

            if (filtros.IdCategoria > 0 || !string.IsNullOrWhiteSpace(filtros.NombreCategoria))
                articulo.Categorias.Add(new CategoriaDeArticulo() { Id = filtros.IdCategoria, Nombre = filtros.NombreCategoria });

            if (filtros.Atributos != null && filtros.Atributos.Any())
            {
                foreach (var atributoDto in filtros.Atributos)
                {
                    var atributo = MapperEntidadDto.Mapper(atributoDto, new Atributo());
                    articulo.Atributos.Add(atributo);
                }
            }

            if (filtros.Codigo != null)
                articulo.Codigos.Add(new CodigoDeBarras() { EAN = filtros.Codigo });

            articulo.Nombre = filtros.Nombre;
            articulo.SKU = filtros.SKU;
            articulo.Activo = filtros.Activo;

            return articulo;
        }

        private static void MapearArticulosSimplesDto(ArticulosPaginados articulos, PagedResponse<GetArticulosSimpleResponse> dto)
        {
            foreach (var articulo in articulos.Articulos)
            {
                var articuloDto = MapperEntidadDto.Mapper(articulo, new GetArticulosSimpleResponse());

                articuloDto.Archivos = new List<ArchivoDto>();
                foreach (var archivo in articulo.Archivos)
                {
                    var item = MapperEntidadDto.Mapper(archivo, new ArchivoDto());
                    item.Tipo = archivo.Tipo.Nombre;
                    articuloDto.Archivos.Add(item);
                }

                articuloDto.Codigos = articulo.Codigos.Select(x => x.EAN).ToList();
                dto.List.Add(articuloDto);
            }
        }
        #endregion

    }
}
