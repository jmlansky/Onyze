using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Pubilicidades;
using Touch.Core.Publicaciones;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Publicaciones;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Articulos;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Publicaciones
{
    public class PublicacionesService : SingleEntityComunService<Publicacion>, IPublicacionesService
    {
        private readonly IPublicacionesRepository publicacionesRepository;
        private readonly IArchivosService archivosService;
        private readonly ISingleEntityComunRepository<TipoObjetoPublicitar> tipoObjetoPublicitarRepository;
        private readonly IObjetoAPublicitarRepository objetosRepository;

        private readonly ISingleEntityComunRepository<Gondola> gondolasRepository;
        private readonly IArticulosService articulosService;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;

        public PublicacionesService(IPublicacionesRepository publicacionesRepository,
            ISingleEntityComunRepository<TipoObjetoPublicitar> tipoObjetoPublicitarRepository,
            IArchivosService archivosService,
            ISingleEntityComunRepository<Gondola> gondolasRepository,
            IObjetoAPublicitarRepository objetosRepository,
            IArticulosService articulosService,
            ICategoriasDeArticuloRepository categoriasDeArticuloRepository) : base(publicacionesRepository)
        {
            this.publicacionesRepository = publicacionesRepository;
            this.archivosService = archivosService;
            this.tipoObjetoPublicitarRepository = tipoObjetoPublicitarRepository;
            this.gondolasRepository = gondolasRepository;
            this.articulosService = articulosService;
            this.objetosRepository = objetosRepository;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
        }

        public async override Task<ServiceResult> Insert(Publicacion entity)
        {
            var existe = await publicacionesRepository.Get(entity.Nombre);
            if (existe != null && existe.Any())
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Publicacion");

            var result = new ServiceResult()
            {
                HasErrors = false,
                StatusCode = ServiceMethodsStatusCode.Ok,
                Message = "Publicación - Se ha insertado correctamente",
                Method = "Insert"
            };

            var columnsToIgnore = new string[] { "ObjetosAPublicitar" };
            if (entity.Archivo == null)
                columnsToIgnore = columnsToIgnore.Append("Archivo").ToArray();

            entity.Creado = DateTime.Now;
            result.IdObjeto = await publicacionesRepository.InsertAndGetId(entity, columnsToIgnore);

            if (result.IdObjeto <= 0)
                return GetServiceResult(ServiceMethod.Insert, "Publicación", false);

            return result;
        }

        public override async Task<ServiceResult> Update(Publicacion entity)
        {
            var existe = await publicacionesRepository.Get(entity.Id);
            if (existe == null || existe.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Publicacion");

            entity.Modificado = DateTime.Now;
            var result = GetServiceResult(ServiceMethod.Update, "Publicación", await publicacionesRepository.Update(entity, new string[] { "Archivo", "ObjetosAPublicitar" }));

            return result;
        }

        public override Task<IEnumerable<Publicacion>> Get()
        {
            var publicaciones = publicacionesRepository.Get().Result;
            var tiposDeObjetos = tipoObjetoPublicitarRepository.Get().Result;

            foreach (var publicacion in publicaciones)
                CompletarItemsDePubliaccion(publicacion, tiposDeObjetos);

            return Task.FromResult(publicaciones);
        }

        public override Task<Publicacion> Get(long id)
        {
            var result = new Publicacion();
            var t = Task.Run(() =>
            {
                result = publicacionesRepository.Get(id).Result;
                var tiposDeObjetos = tipoObjetoPublicitarRepository.Get().Result;

                CompletarItemsDePubliaccion(result, tiposDeObjetos);

            });
            t.Wait();

            return Task.FromResult(result);
        }

        public override async Task<IEnumerable<Publicacion>> Get(string name)
        {
            var items = await publicacionesRepository.Get(name);
            var tiposDeObjetos = await tipoObjetoPublicitarRepository.Get();
            foreach (var item in items)
                CompletarItemsDePubliaccion(item, tiposDeObjetos);

            return items;
        }

        private void CompletarItemsDePubliaccion(Publicacion item, IEnumerable<TipoObjetoPublicitar> tiposDeObjetos)
        {

            if (item.IdArchivo > 0)
                item.Archivo = archivosService.Get(item.IdArchivo).Result;

            var objetosAPublicar = objetosRepository.GetFromPublicacion(item.Id, new string[] { "Tipo", "Objeto" }).Result;
            foreach (var objeto in objetosAPublicar)
            {
                objeto.Tipo = tiposDeObjetos.FirstOrDefault(x => x.Id == objeto.IdTipo);

                if (objeto.Tipo != null)
                {
                    objeto.IdTipo = objeto.Tipo.Id;
                    if (objeto.Tipo != null && !string.IsNullOrWhiteSpace(objeto.Tipo.Tags))
                    {
                        if (objeto.Tipo.Tags.ToLower().Contains("góndola"))
                            objeto.Objeto = gondolasRepository.Get(objeto.IdObjeto, new string[] { "Estantes" }).Result;

                        if (objeto.Tipo.Tags.ToLower().Contains("artículo"))
                            objeto.Objeto = articulosService.GetArticulo(objeto.IdObjeto, false).Result;

                        if (objeto.Tipo.Tags.ToLower().Contains("categoria"))
                            objeto.Objeto = categoriasDeArticuloRepository.Get(objeto.IdObjeto).Result;

                        if (objeto.Objeto != null)
                            item.ObjetosAPublicitar.Add(objeto);
                    }
                }
            }
        }

        public async Task<ServiceResult> DeleteObjectsFromPantalla(List<ObjetoAPublicar> objetos)
        {
            var result = GetServiceResult(ServiceMethod.Delete, "Objetos de publicación", true);
            foreach (var item in objetos)
            {
                item.Modificado = DateTime.Now;
                if (!(await objetosRepository.Delete(item)))
                {
                    result.HasErrors = true;
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    result.Message = "No se pudo eliminar alguno/s de los objetos que intenta eliminar";
                }
            }
            return result;
        }

        public async Task<ServiceResult> InsertObjeto(ObjetoAPublicar item)
        {
            var existe = await objetosRepository.GetFromPublicacion(item.IdPantalla, new string[] { "Objeto", "Tipo" });
            if (existe != null && existe.Any(x => x.IdObjeto == item.IdObjeto))
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Objeto de la publicación");

            item.Creado = DateTime.Now;
            var result = GetServiceResult(ServiceMethod.Insert, "Objeto de la publicación", await objetosRepository.Insert(item, new string[] { "Objeto", "Tipo" }));

            return result;
        }
    }
}
