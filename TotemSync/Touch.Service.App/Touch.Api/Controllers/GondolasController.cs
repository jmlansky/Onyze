using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Estantes;
using Touch.Api.Dtos.Gondolas;
using Framework.Helpers;
using Touch.Core.Gondolas;
using Touch.Service.Gondolas;
using Touch.Api.Dtos;
using PagedList;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Touch.Api.Dtos.Grilla;
using Microsoft.Extensions.Configuration;
using Touch.Core.Archivos;
using Touch.Repositories.Articulos.Contracts;
using Touch.Api.Dtos.CategoriasDeArticulo;
using Touch.Repositories.Archivos.Contracts;
using Touch.Api.Dtos.Archivos;
using Touch.Core.Comun;
using Touch.Api.Dtos.CodigosDeBarra;

namespace Touch.Api.Controllers
{

    public class GondolasController : BaseController
    {
        private readonly IGondolasService gondolasService;


        public GondolasController(IConfiguration configuration, IGondolasService gondolasService) : base(configuration)
        {
            this.gondolasService = gondolasService;

        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {

            var result = await gondolasService.Get(pageNumber, pageSize);




            var response = new PagedResponse<GondolaDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = MappearGondolaADto((PagedList<Gondola>)result.PagedList);

            return Ok(response);
        }

        [HttpGet("GetWithDeleted")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetWithDeleted(int? pageNumber, int? pageSize, string fechaSincro)
        {

            var result = await gondolasService.GetWithDeleted(pageNumber, pageSize, fechaSincro);


            var current = await gondolasService.GetCurentCount();

            var response = new PagedResponse<GondolaAllDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords,
                CurrentRecords = current
            };

            response.List = MappearGondolaAllADto((PagedList<Gondola>)result.PagedList);

            return Ok(response);
        }

        private List<GondolaAllDto> MappearGondolaAllADto(PagedList<Gondola> pagedList)
        {
            var list = new List<GondolaAllDto>();
            foreach (var gondola in pagedList)
            {
                var gondolaDto = MapperEntidadDto.Mapper(gondola, new GondolaAllDto());

                if (gondola.Categoria != null)
                    gondolaDto.Categoria = MapperEntidadDto.Mapper(gondola.Categoria, new CategoriaDto());



                gondolaDto.Grilla = MapperEntidadDto.Mapper(gondola.Grilla, new GrillaDeGondolaDto());

                foreach (var estante in gondola.Estantes)
                {
                    var estanteDto = MapperEntidadDto.Mapper(estante, new EstanteAllDto());
                    estanteDto.Articulos = estante.Articulos.Select(x => MapperEntidadDto.Mapper(x, new ArticuloEstanteAllDto())).ToList();
                    gondolaDto.Estantes.Add(estanteDto);
                }

                list.Add(gondolaDto);
            }

            return list;
        }

        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre, int? pageNumber, int? pageSize)
        {
            if (string.IsNullOrEmpty(nombre))
                return BadRequest("Por favor ingrese el nombre de la góndola que quiere buscar");

            var result = await gondolasService.Get(nombre, pageNumber, pageSize);


            var response = new PagedResponse<GondolaDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = MappearGondolaADto((PagedList<Gondola>)result.PagedList);

            return Ok(response);
        }



        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await gondolasService.Get(id);
            if (result == null || result.Id == 0)
                return NotFound("No se ha encontrado la góndola que busca.");

            var gondola = MappearGondolaADto(result);



            return Ok(gondola);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostGondolaDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Titulo) || string.IsNullOrEmpty(dto.Nombre))
                    return BadRequest("Por favor ingrese título y nombre");

                var gondola = MapperEntidadDto.Mapper(dto, new Gondola());

                if (dto.Grilla != null)
                    gondola.Grilla = MapperEntidadDto.Mapper(dto.Grilla, new Grilla());

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    gondola.Nombre = gondola.Titulo;

                var result = await gondolasService.Insert(gondola);

                return StatusCode((int)result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar la góndola: " + ex.Message);
            }
        }

        [HttpPost("ConEstantes")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PostGondolaConEstantes([FromBody] PostGondolaConEstantesDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Titulo) || string.IsNullOrEmpty(dto.Nombre))
                    return BadRequest("Por favor ingrese título y nombre");

                var gondola = MapperEntidadDto.Mapper(dto, new Gondola());

                if (dto.Grilla != null)
                    gondola.Grilla = MapperEntidadDto.Mapper(dto.Grilla, new Grilla());

                foreach (var estanteDto in dto.Estantes)
                {
                    var estante = MapperEntidadDto.Mapper(estanteDto, new Estante());
                    //estante.Articulos = estanteDto.Articulos.Select(x => MapperEntidadDto.Mapper(x, new ArticuloEstante())).ToList();

                    foreach (var art in estanteDto.Articulos)
                    {
                        var articulo = MapperEntidadDto.Mapper(art, new ArticuloEstante());

                        if (art.Decoraciones != null)
                            articulo.Decoraciones = art.Decoraciones.Select(x => MapperEntidadDto.Mapper(x, new ArticuloDecoracion())).ToList();

                        estante.Articulos.Add(articulo);

                    }
                    if (estanteDto.Decoraciones != null && estanteDto.Decoraciones.Any())
                        MapearDecoraciones(estanteDto.Decoraciones, estante);
                    gondola.Estantes.Add(estante);
                }

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    gondola.Nombre = gondola.Titulo;

                var result = await gondolasService.InsertGondolaConEstantes(gondola);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar la góndola: " + ex.Message);
            }
        }

        [HttpPut("{id}/ConEstantes")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> PutGondolaConEstantes(long id, [FromBody] PutGondolaConEstantesDto dto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido");

                if (string.IsNullOrEmpty(dto.Titulo) || string.IsNullOrEmpty(dto.Nombre))
                    return BadRequest("Por favor ingrese título y nombre");

                var gondola = MapperEntidadDto.Mapper(dto, new Gondola());
                gondola.Id = id;

                if (dto.Grilla != null)
                    gondola.Grilla = MapperEntidadDto.Mapper(dto.Grilla, new Grilla());

                foreach (var estanteDto in dto.Estantes)
                {
                    var estante = MapperEntidadDto.Mapper(estanteDto, new Estante());
                    //estante.Articulos = estanteDto.Articulos.Select(x => MapperEntidadDto.Mapper(x, new ArticuloEstante())).ToList();

                    foreach (var art in estanteDto.Articulos)
                    {
                        var articulo = MapperEntidadDto.Mapper(art, new ArticuloEstante());

                        if (art.Decoraciones != null)
                            articulo.Decoraciones = art.Decoraciones.Select(x => MapperEntidadDto.Mapper(x, new ArticuloDecoracion())).ToList();

                        estante.Articulos.Add(articulo);

                    }
                    if (estanteDto.Decoraciones != null && estanteDto.Decoraciones.Any())
                        MapearDecoraciones(estanteDto.Decoraciones, estante);
                    gondola.Estantes.Add(estante);
                }

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    gondola.Nombre = gondola.Titulo;

                var result = await gondolasService.UpdateGondolaConEstantes(gondola);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al insertar la góndola: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutGondolaDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            try
            {
                var gondola = MapperEntidadDto.Mapper(dto, new Gondola());
                gondola.Id = id;
                gondola.Modificado = DateTime.Now;
                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    gondola.Nombre = gondola.Titulo;

                if (gondola.Grilla != null)
                    gondola.Grilla = MapperEntidadDto.Mapper(dto.grilla, new Grilla());

                var result = await gondolasService.Update(gondola);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al actualizar la góndola: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            try
            {
                var gondola = new Gondola() { Id = id, Modificado = DateTime.Now };
                var result = await gondolasService.Delete(gondola);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al actualizar la góndola: " + ex.Message);
            }
        }

        private GondolaDto MappearGondolaADto(Gondola gondola)
        {
            var gondolaDto = MapperEntidadDto.Mapper(gondola, new GondolaDto());
            gondolaDto.Grilla = MapperEntidadDto.Mapper(gondola.Grilla, new GrillaDeGondolaDto());

            if (gondola.Categoria != null)
                gondolaDto.Categoria = MapperEntidadDto.Mapper(gondola.Categoria, new CategoriaDto());

            foreach (var estante in gondola.Estantes)
            {
                var estanteDto = MapperEntidadDto.Mapper(estante, new EstanteDto());
                if (estante.Decoraciones.Any())
                {
                    foreach (var deco in estante.Decoraciones)
                    {
                        var decoDto = MapperEntidadDto.Mapper(deco, new DecoracionEstanteDto());
                        if (deco.Archivo != null)
                            decoDto.Archivo = MapperEntidadDto.Mapper(deco.Archivo, new ArchivoDto());

                        estanteDto.Decoraciones.Add(decoDto);
                    }

                }




                foreach (var art in estante.Articulos)
                {
                    var articuloDto = MapperEntidadDto.Mapper(art, new ArticuloEstanteDto());
                    if (art.Decoraciones.Any())
                    {
                        articuloDto.Decoraciones = new List<ArticuloDecoracionDto>();
                        foreach (var deco in art.Decoraciones)
                        {
                            var decoDto = MapperEntidadDto.Mapper(deco, new ArticuloDecoracionDto());
                            if (deco.Archivo != null)
                                decoDto.Archivo = MapperEntidadDto.Mapper(deco.Archivo, new ArchivoDto());


                            articuloDto.Decoraciones.Add(decoDto);
                        }
                    }

                    if (art.CodigosDeBarra.Any())
                    {
                        articuloDto.CodigosDeBarra = new List<CodigoDeBarrasDto>();
                        foreach (var cod in art.CodigosDeBarra)
                        {
                            var codDto = MapperEntidadDto.Mapper(cod, new CodigoDeBarrasDto());

                            articuloDto.CodigosDeBarra.Add(codDto);
                        }
                    }


                    estanteDto.Articulos.Add(articuloDto);
                }
                gondolaDto.Estantes.Add(estanteDto);
            }

            return gondolaDto;
        }

        private List<GondolaDto> MappearGondolaADto(PagedList<Gondola> pagedList)
        {
            var list = new List<GondolaDto>();
            foreach (var gondola in pagedList)
            {
                var gondolaDto = MapperEntidadDto.Mapper(gondola, new GondolaDto());

                if (gondola.Categoria != null)
                    gondolaDto.Categoria = MapperEntidadDto.Mapper(gondola.Categoria, new CategoriaDto());


                gondolaDto.Grilla = MapperEntidadDto.Mapper(gondola.Grilla, new GrillaDeGondolaDto());

                foreach (var estante in gondola.Estantes)
                {
                    var estanteDto = MapperEntidadDto.Mapper(estante, new EstanteDto());
                    estanteDto.Articulos = estante.Articulos.Select(x => MapperEntidadDto.Mapper(x, new ArticuloEstanteDto())).ToList();
                    gondolaDto.Estantes.Add(estanteDto);
                }

                list.Add(gondolaDto);
            }

            return list;
        }

        private void MapearDecoraciones(List<PostDecoracionEstanteDto> decoracionesDto, Estante estante)
        {
            foreach (var decoDto in decoracionesDto)
            {
                var deco = MapperEntidadDto.Mapper(decoDto, new EstanteDecoracion());


                estante.Decoraciones.Add(deco);
            }
        }
    }
}
