using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos.Archivos;
using Touch.Api.Dtos.Gondolas;
using Touch.Api.Dtos.TiposMultimedia;
using Framework.Helpers;
using Touch.Core.Comun;
using Touch.Api.Dtos;
using PagedList;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Touch.Service.Playlists;
using Touch.Api.Dtos.Playlists;
using Touch.Core.Playlists;
using Touch.Api.Dtos.Articulos;
using Touch.Api.Dtos.Publicaciones;
using Touch.Api.Dtos.Sectores;
using System;

namespace Touch.Api.Controllers
{
    public class PlaylistsController : BaseController
    {
        private readonly IPlaylistsService playlistsService;
        public PlaylistsController(IConfiguration configuration, IPlaylistsService playlistsService) : base(configuration)
        {
            this.playlistsService = playlistsService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await playlistsService.Get(pageNumber, pageSize);
            var response = new PagedResponse<GetPlaylistsDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Playlist>)result.PagedList).Select(x => MapearPlaylistADto(x)).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await playlistsService.Get(id);

            if (result == null || result.Id <= 0)
                return NotFound();

            return Ok(MapearPlaylistADto(result));
        }

        [HttpGet("nombre/{nombre}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(string nombre, int? pageNumber, int? pageSize)
        {
            var result = await playlistsService.Get(nombre, pageNumber, pageSize);
            var response = new PagedResponse<GetPlaylistsDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Playlist>)result.PagedList).Select(x => MapearPlaylistADto(x)).ToList();

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostPlaylistsDto playlistDto)
        {

            var playlist = MapperEntidadDto.Mapper(playlistDto, new Playlist());

            if (playlistDto.Multimedia != null && playlistDto.Multimedia.Any())
                playlist.Multimedia = playlistDto.Multimedia.Select(x => MapperEntidadDto.Mapper(x, new Multimedia())).ToList();


            if (playlistDto.Sectores != null && playlistDto.Sectores.Any())
                playlist.PlaylistDeSector = playlistDto.Sectores.Select(x => MapperEntidadDto.Mapper(x, new PlaylistDeSector())).ToList();

            var result = await playlistsService.Insert(playlist);

            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutPlaylistsDto playlistDto)
        {
            try
            {
                if (id <= 0)
                    return BadRequest("Por favor ingrese un id válido");

                if (string.IsNullOrEmpty(playlistDto.Nombre))
                    return BadRequest("Por favor ingrese un nombre");

                var playlist = MapperEntidadDto.Mapper(playlistDto, new Playlist());
                playlist.Id = id;



                if (playlistDto.Multimedia != null && playlistDto.Multimedia.Any())
                    playlist.Multimedia = playlistDto.Multimedia.Select(x => MapperEntidadDto.Mapper(x, new Multimedia())).ToList();


                if (playlistDto.Sectores != null && playlistDto.Sectores.Any())
                    playlist.PlaylistDeSector = playlistDto.Sectores.Select(x => MapperEntidadDto.Mapper(x, new PlaylistDeSector())).ToList();

                var result = await playlistsService.Update(playlist);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest("Hubo un error al actualizar la playlist: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            var result = await playlistsService.Delete(new Playlist() { Id = id });
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        private static GetPlaylistsDto MapearPlaylistADto(Playlist item)
        {
            var dtoItem = MapperEntidadDto.Mapper(item, new GetPlaylistsDto());




            foreach (var objeto in item.Multimedia)
            {
                var objetoDto = new GetMultimediaDto();

                if (objeto.Objeto.GetType().Name == "Gondola") {
                    objetoDto.Objeto = MapperEntidadDto.Mapper(objeto.Objeto, new GondolaDto());
                    
                }


                if (objeto.Objeto.GetType().Name == "Articulo")
                    objetoDto.Objeto = MapperEntidadDto.Mapper(objeto.Objeto, new ArticuloDto());

                if (objeto.Objeto.GetType().Name == "Publicacion")
                    objetoDto.Objeto = MapperEntidadDto.Mapper(objeto.Objeto, new PublicacionDto());


                objetoDto.Tipo = MapperEntidadDto.Mapper(objeto.Tipo, new TipoMultimediaDto());
                objetoDto.Tiempo = objeto.Tiempo;
                objetoDto.url = objeto.Url;


                dtoItem.Multimedia.Add(objetoDto);
            }


            foreach (var objeto in item.Sector)
            {
                var objetoDto = MapperEntidadDto.Mapper(objeto, new SectorDto());

                dtoItem.Sectores.Add(objetoDto);

            }

            return dtoItem;
        }
    }
}
