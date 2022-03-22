using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Playlists;
using Touch.Core.Publicaciones;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Playlists;
using Touch.Repositories.Publicaciones;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Articulos;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Playlists
{
    public class PlaylistsService : SingleEntityComunService<Playlist>, IPlaylistsService
    {
        private readonly IPlaylistsRepository playlistsRepository;
        private readonly IArchivosService archivosService;
        private readonly ISingleEntityComunRepository<TipoMultimedia> tipoMultimediaRepository;
        private readonly IMultimediaRepository multimediaRepository;

        private readonly ISingleEntityComunRepository<Publicacion> publicacionesRepository;

        private readonly ISingleEntityComunRepository<Gondola> gondolasRepository;
        private readonly IArticulosService articulosService;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;

        private readonly IPlaylistDeSectorRepository playlistDeSectorRepository;
        private readonly ISingleEntityComunRepository<Sector> sectoresRepository;
        private readonly IArchivosRepository archivosRepository;

        public PlaylistsService(IPlaylistsRepository playlistsRepository,
            ISingleEntityComunRepository<TipoMultimedia> tipoMultimediaRepository,
            IArchivosService archivosService,
            ISingleEntityComunRepository<Gondola> gondolasRepository,
            IMultimediaRepository multimediaRepository,
            IArticulosService articulosService,
            ICategoriasDeArticuloRepository categoriasDeArticuloRepository,
            ISingleEntityComunRepository<Publicacion> publicacionesRepository,
            IPlaylistDeSectorRepository playlistDeSectorRepository,
            ISingleEntityComunRepository<Sector> sectoresRepository,
            IArchivosRepository archivosRepository) : base(playlistsRepository)
        {
            this.playlistsRepository = playlistsRepository;
            this.archivosService = archivosService;
            this.tipoMultimediaRepository = tipoMultimediaRepository;
            this.gondolasRepository = gondolasRepository;
            this.articulosService = articulosService;
            this.multimediaRepository = multimediaRepository;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
            this.publicacionesRepository = publicacionesRepository;
            this.playlistDeSectorRepository = playlistDeSectorRepository;
            this.sectoresRepository = sectoresRepository;
            this.archivosRepository = archivosRepository;

        }

        public async override Task<ServiceResult> Insert(Playlist entity)
        {
            var existe = await playlistsRepository.Get(entity.Nombre);
            if (existe != null && existe.Any())
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Playlist");

            var result = new ServiceResult()
            {
                HasErrors = false,
                StatusCode = ServiceMethodsStatusCode.Ok,
                Message = "Playlist - Se ha insertado correctamente",
                Method = "Insert"
            };

            var columnsToIgnore = new string[] { "Multimedia", "Sectores", "PlaylistDeSector", "Sector", "Url" };

            entity.Creado = DateTime.Now;
            result.IdObjeto = await playlistsRepository.InsertAndGetId(entity, columnsToIgnore);

            if (result.IdObjeto <= 0)
                return GetServiceResult(ServiceMethod.Insert, "Playlist", false);

            return result;
        }

        public override async Task<ServiceResult> Update(Playlist entity)
        {
            var existe = await playlistsRepository.Get(entity.Id);
            if (existe == null || existe.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Playlist");

            var existeNombre = await playlistsRepository.Get(entity.Nombre);
            if (existeNombre != null && existeNombre.Any())
                foreach (var item in existeNombre)
                    if (item.Id != entity.Id)
                        return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Playlist");


            entity.Modificado = DateTime.Now;
            var result = GetServiceResult(ServiceMethod.Update, "Playlist", await playlistsRepository.Update(entity, new string[] { "Multimedia", "Sectores", "Sector", "PlaylistDeSector", "Url" }));

            return result;




        }

        public override Task<IEnumerable<Playlist>> Get()
        {
            var playlists = playlistsRepository.Get().Result;
            var tiposDeObjetos = tipoMultimediaRepository.Get().Result;

            foreach (var playlist in playlists)
            {
                CompletarItemsDePlaylist(playlist, tiposDeObjetos);


                List<PlaylistDeSector> Sectores = playlistDeSectorRepository.GetFromPlaylist(playlist.Id, new string[] { "Multimedia", "Sectores", "Objeto", "Tipo", "Url" }).Result.ToList();

                foreach (var playlistDeSector in Sectores)
                {
                    var sector = sectoresRepository.Get(playlistDeSector.IdSector).Result;
                    if (sector != null && sector.Id > 0)
                        playlist.Sector.Add(sector);
                }

            }


            return Task.FromResult(playlists);
        }

        public override Task<Playlist> Get(long id)
        {
            var playlist = new Playlist();
            var t = Task.Run(() =>
            {
                playlist = playlistsRepository.Get(id).Result;
                var tiposDeObjetos = tipoMultimediaRepository.Get().Result;


                CompletarItemsDePlaylist(playlist, tiposDeObjetos);


                List<PlaylistDeSector> Sectores = playlistDeSectorRepository.GetFromPlaylist(playlist.Id, new string[] { "Multimedia", "Sectores", "Objeto", "Tipo", "Url" }).Result.ToList();

                foreach (var playlistDeSector in Sectores)
                {
                    var sector = sectoresRepository.Get(playlistDeSector.IdSector).Result;
                    if (sector != null && sector.Id > 0)
                        playlist.Sector.Add(sector);
                }



            });
            t.Wait();

            return Task.FromResult(playlist);
        }

        public override async Task<IEnumerable<Playlist>> Get(string name)
        {
            var items = await playlistsRepository.Get(name);
            var tiposDeObjetos = await tipoMultimediaRepository.Get();
            foreach (var item in items)
                CompletarItemsDePlaylist(item, tiposDeObjetos);

            return items;
        }

        private void CompletarItemsDePlaylist(Playlist item, IEnumerable<TipoMultimedia> tiposMultimedia)
        {






            var objetosAPublicar = multimediaRepository.GetFromPlaylist(item.Id, new string[] { "Multimedia", "Sectores", "Objeto", "Tipo", "Url" }).Result;
            foreach (var objeto in objetosAPublicar)
            {
                objeto.Tipo = tiposMultimedia.FirstOrDefault(x => x.Id == objeto.IdTipo);

                if (objeto.Tipo != null)
                {
                    objeto.IdTipo = objeto.Tipo.Id;
                    if (objeto.Tipo != null && !string.IsNullOrWhiteSpace(objeto.Tipo.Nombre))
                    {
                        if (objeto.Tipo.Tags.ToLower().Contains("góndola"))
                        {
                            Gondola g = gondolasRepository.Get(objeto.IdObjeto, new string[] { "Estantes", "Grilla",  "Image", "Categoria" }).Result;

                            objeto.Objeto = g;
                            objeto.Url = g.Id + "_" + g.Imagen.ToString("yyyyMMddHHmmss") + ".jpg";
                        }

                        else if (objeto.Tipo.Tags.ToLower().Contains("publicacion"))
                        {
                            Publicacion publicacion = publicacionesRepository.Get(objeto.IdObjeto, new string[] { "ObjetosAPublicitar", "Archivo" }).Result;

                            objeto.Objeto = publicacion;
                            objeto.Url = archivosRepository.Get(publicacion.IdArchivo, new string[] { "Tipo", "IdTipo", "NombreGuardado", "IdArticulo", "IdArchivoOriginal", "Size", "File", "Miniaturas", "ColorPromedio", "Width", "Height", "Id", "Nombre", "Creado", "Modificado" }).Result.Url;

                        }

                        else if (objeto.Tipo.Tags.ToLower().Contains("categoria"))
                            objeto.Objeto = categoriasDeArticuloRepository.Get(objeto.IdObjeto).Result;

                        if (objeto.Objeto != null)
                            item.Multimedia.Add(objeto);
                    }
                }
            }
        }

        public async Task<ServiceResult> DeleteObjectsFromPantalla(List<Multimedia> objetos)
        {
            var result = GetServiceResult(ServiceMethod.Delete, "Objetos de publicación", true);
            foreach (var item in objetos)
            {
                item.Modificado = DateTime.Now;
                if (!(await multimediaRepository.Delete(item)))
                {
                    result.HasErrors = true;
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    result.Message = "No se pudo eliminar alguno/s de los objetos que intenta eliminar";
                }
            }
            return result;
        }

        public override async Task<ServiceResult> Delete(Playlist entity)
        {
            var result = await base.Delete(entity);

            if (!result.HasErrors)
            {
                var resultMultimedia = await multimediaRepository.DeleteFromPlaylist(entity.Id);

                if (!resultMultimedia)
                {
                    ServiceResult.HasErrors = true;
                    ServiceResult.Message = "Ha ocurrido un problema al eliminar multimedia de las playlists";
                    ServiceResult.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    ServiceResult.Method = ServiceMethod.Delete.ToString();
                    return result;
                }

                var resultSectores = await playlistDeSectorRepository.DeleteFromPlaylist(entity.Id);

                if (!resultSectores)
                {
                    ServiceResult.HasErrors = true;
                    ServiceResult.Message = "Ha ocurrido un problema al eliminar los sectores de las playlists";
                    ServiceResult.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    ServiceResult.Method = ServiceMethod.Delete.ToString();
                    return result;
                }



            }

            return result;
        }

        public async Task<ServiceResult> InsertObjeto(Multimedia item)
        {
            var existe = await multimediaRepository.GetFromPlaylist(item.IdPlaylist, new string[] { "Multimedia", "Sectores" });
            if (existe != null && existe.Any(x => x.IdObjeto == item.IdObjeto))
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Objeto de la publicación");

            item.Creado = DateTime.Now;
            var result = GetServiceResult(ServiceMethod.Insert, "Multimedia de la playlist", await multimediaRepository.Insert(item, new string[] { "Multimedia", "Sectores" }));

            return result;
        }
    }
}
