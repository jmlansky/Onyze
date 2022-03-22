using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Gondolas.Contracts;
using Touch.Repositories.Publicaciones;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Gondolas
{
    public class GondolasService : SingleEntityComunService<Gondola>, IGondolasService
    {
        private readonly ICodigosDeBarraRepository codigosDeBarraRepo;
        private readonly IGondolasRepository gondolasRepository;
        private readonly IEstantesRepository estantesRepository;
        private readonly IArticulosRepository articulosRepository;
        private readonly ITipoObjetoPublicitarRepository tipoObjetoPublicitarRepository;
        private readonly IPublicacionesRepository publicacionesRepository;
        private readonly IArticulosPorEstanteRepository articulosPorEstanteRepository;
        private readonly IArchivosRepository archivosRepository;
        private readonly IEstantesDecoracionesRepository decoracionesEstantesRepository;
        private readonly IGrillasRepository grillasRepository;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;
        public readonly IArticulosDecoracionesRepository articulosDecoracionesRepository;

        //private string[] columnsToIgnoreGondola = { "Estantes", "Articulos", "Grilla" };

        public GondolasService(IGondolasRepository gondolasRepository,
            IEstantesRepository estantesRepository,
            ITipoObjetoPublicitarRepository tipoObjetoPublicitarRepository,
            IPublicacionesRepository publicacionesRepository,
            IArticulosPorEstanteRepository articulosPorEstanteRepository,
            IArticulosRepository articulosRepository,
            IArchivosRepository archivosRepository,
            IEstantesDecoracionesRepository decoracionesEstantesRepository,
            IGrillasRepository grillasRepository,
            IArticulosDecoracionesRepository articulosDecoracionesRepository,
            ICodigosDeBarraRepository codigosDeBarraRepo,
            ICategoriasDeArticuloRepository categoriasDeArticuloRepository) : base(gondolasRepository)
        {
            this.gondolasRepository = gondolasRepository;
            this.estantesRepository = estantesRepository;
            this.tipoObjetoPublicitarRepository = tipoObjetoPublicitarRepository;
            this.publicacionesRepository = publicacionesRepository;
            this.articulosPorEstanteRepository = articulosPorEstanteRepository;
            this.articulosRepository = articulosRepository;
            this.archivosRepository = archivosRepository;
            this.decoracionesEstantesRepository = decoracionesEstantesRepository;
            this.grillasRepository = grillasRepository;
            this.articulosDecoracionesRepository = articulosDecoracionesRepository;
            this.codigosDeBarraRepo = codigosDeBarraRepo;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
        }

        public override async Task<ServiceResult> Insert(Gondola gondola)
        {
            var existentes = await gondolasRepository.Get(gondola.Nombre, new string[] { "Estantes", "Articulos", "Grilla", "Image", "Categoria" });
            var existe = existentes.Any(x => x.Nombre.ToLower().Equals(gondola.Nombre.ToLower()));
            if (existe)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Góndola");

            if (gondola.IdEncabezado > 0 && !await ValidarExisteArchivo(gondola.IdEncabezado))
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Archivo de Encabezado");

            if (gondola.IdFondo > 0 && !await ValidarExisteArchivo(gondola.IdFondo))
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Archivo de Fondo");

            gondola.Creado = DateTime.Now;
            var id = await gondolasRepository.InsertAndGetId(gondola, new string[] { "Estantes", "Articulos", "Grilla", "Imagen", "Image", "Categoria" });
            var result = GetServiceResult(ServiceMethod.Insert, "Góndola", id > 0);
            result.IdObjeto = id;

            return result;
        }

        public async Task<ServiceResult> InsertGondolaConEstantes(Gondola gondola)
        {
            var result = new ServiceResult();
            try
            {
                var t = Task.Run(() =>
                {
                    var existentes = gondolasRepository.Get(gondola.Nombre, new string[] { "Estantes", "Articulos", "Grilla", "Image", "Categoria" });
                    var existe = existentes.Result.Any(x => x.Nombre.ToLower().Equals(gondola.Nombre.ToLower()));
                    if (existe)
                        result = GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Góndola");

                    var ids = new List<long>();
                    if (gondola.IdFondo > 0 && !ValidarExisteArchivo(gondola.IdFondo).Result)
                        ids.Add(gondola.IdFondo);
                    if (gondola.IdEncabezado > 0 && !ValidarExisteArchivo(gondola.IdEncabezado).Result)
                        ids.Add(gondola.IdEncabezado);

                    if (ids.Any())
                    {
                        var archivos = archivosRepository.Get(ids, new string[] { "File", "Miniaturas", "Tipo" }).Result.ToList();
                        if (!archivos.Any())
                            throw new Exception("El archivo de fondo o de encabezado no existe");
                    }

                    if (gondola.Estantes.Any())
                    {
                        var estantes = estantesRepository.Get(new string[] { "Articulos", "Decoraciones" }).Result.TakeWhile(x => gondola.Estantes.Select(z => z.Id).Contains(x.Id));
                        if (estantes != null && estantes.Any())
                            throw new Exception("El estante que desea insertar ya existe");

                        foreach (var estante in gondola.Estantes)
                        {
                            if (estante.Articulos.Any())
                            {
                                var articulos = articulosRepository.GetPorIds(estante.Articulos.Select(x => x.IdArticulo).ToList()).Result;
                                if (!articulos.Any())
                                    throw new Exception("No existe alguno de los artículos que desea insertar");

                                if (articulos.Count() < estante.Articulos.Select(x => x.Id).Distinct().Count())
                                    throw new Exception("No existe alguno de los artículos que desea insertar");

                            }
                        }
                    }
                });

                t.Wait();

                if (!result.HasErrors)
                {
                    gondola.Creado = DateTime.Now;
                    gondola.Grilla.Creado = gondola.Creado;
                    var id = await gondolasRepository.InsertGondaEstantesArticulos(gondola, new string[] { "Estantes", "Articulos", "Grilla", "Imagen", "Image", "Categoria" });
                    result = GetServiceResult(ServiceMethod.Insert, "Góndola", id > 0);
                    result.IdObjeto = id;
                }
            }
            catch (Exception ex)
            {
                result = GetServiceResult(ServiceMethod.Insert, "Gondola - Estante - Articulo", false);
                result.Notes = ex.Message;
            }

            return result;
        }

        public override async Task<ServiceResult> Update(Gondola entity)
        {
            var gondola = await gondolasRepository.Get(entity.Id);
            if (gondola == null || gondola.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Góndola");

            if (entity.IdEncabezado > 0 && !await ValidarExisteArchivo(entity.IdEncabezado))
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Archivo de Encabezado");

            if (entity.IdFondo > 0 && !await ValidarExisteArchivo(entity.IdFondo))
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Archivo de Fondo");

            return GetServiceResult(ServiceMethod.Update, "Góndola", await gondolasRepository.Update(entity, new string[] { "Estantes", "Articulos", "Grilla", "Imagen", "Image", "Categoria" }));
        }

        public override async Task<ServiceResult> Delete(Gondola entity)
        {
            var result = await base.Delete(entity);

            if (!result.HasErrors)
            {
                var resultCascade = await estantesRepository.DeleteFromGondola(entity.Id);

                //Eliminar articulos publicados
                var tipo = await tipoObjetoPublicitarRepository.GetPorTipo("Gondola");
                if (tipo.Any())
                {
                    var idTipoObjetoPublicitar = tipo.FirstOrDefault().Id;
                    if (idTipoObjetoPublicitar > 0)
                        await publicacionesRepository.DeleteObjetosPublicados(entity.Id, idTipoObjetoPublicitar);
                }

                if (!resultCascade)
                {
                    ServiceResult.HasErrors = true;
                    ServiceResult.Message = "Ha ocurrido un problema al eliminar los estantes de la góndola";
                    ServiceResult.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    ServiceResult.Method = ServiceMethod.Delete.ToString();
                }
            }

            return result;
        }

        public override Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, 0);

            var t = Task.Run(() =>
            {
                var gondolas = gondolasRepository.Get(new string[] { "Estantes", "Articulos", "Grilla", "Image", "Categoria", "Archivo", "Categoria" }).Result.ToList().OrderBy(o => o.Nombre).ToList();


                pagedResult.TotalRecords = gondolas.Count();
                var pagedList = new PagedList<Gondola>(gondolas, pageNumber ?? 1, pageSize ?? 25);

                foreach (var gondola in pagedList)
                {
                    gondola.Estantes = estantesRepository.GetEstantesDeLaGondola(gondola.Id).Result.ToList();


                    if (gondola.Imagen != null)
                    {
                        gondola.Image = gondola.Id + "_" + gondola.Imagen.ToString("yyyyMMddHHmmss") + ".jpg";
                    }

                    GetEstantes(gondola);

                    if (gondola.IdCategoria.HasValue)
                        gondola.Categoria = categoriasDeArticuloRepository.Get(gondola.IdCategoria.Value).Result;

                }
                pagedResult.PagedList = pagedList;
                var totalPages = Math.Ceiling((double)(pagedResult.TotalRecords / (double)(pageSize ?? 25)));
                pagedResult.TotalPages = totalPages == 0 ? 1 : totalPages;
            });

            t.Wait();

            return Task.FromResult(pagedResult);
        }


        public Task<PagedResult> GetWithDeleted(int? pageNumber, int? pageSize, string fechaSincro)
        {
            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, 0);

            var t = Task.Run(() =>
            {

                DateTime dateTime = DateTime.ParseExact(fechaSincro, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                var gondolas = gondolasRepository.GetWithDeleted(dateTime).Result.ToList().OrderBy(o => o.Nombre).ToList();


                pagedResult.TotalRecords = gondolas.Count();
                var pagedList = new PagedList<Gondola>(gondolas, pageNumber ?? 1, pageSize ?? 25);

                foreach (var gondola in pagedList)
                {
                    gondola.Estantes = estantesRepository.GetEstantesDeLaGondola(gondola.Id).Result.ToList();
                    GetEstantes(gondola);

                    foreach (var estante in gondola.Estantes)
                        estante.Articulos = articulosPorEstanteRepository.GetArticulosPorEstante(estante.Id).Result.ToList();
                }
                pagedResult.PagedList = pagedList;
                var totalPages = Math.Ceiling((double)(pagedResult.TotalRecords / (double)(pageSize ?? 25)));
                pagedResult.TotalPages = totalPages == 0 ? 1 : totalPages;
            });

            t.Wait();

            return Task.FromResult(pagedResult);
        }

        public override Task<PagedResult>  Get(string nombre, int? pageNumber, int? pageSize)
        {


            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, 0);

            var t = Task.Run(() =>
            {
                var gondolas = gondolasRepository.Get(nombre).Result.ToList();


                pagedResult.TotalRecords = gondolas.Count();
                var pagedList = new PagedList<Gondola>(gondolas, pageNumber ?? 1, pageSize ?? 25);

                foreach (var gondola in pagedList)
                {
                    gondola.Estantes = estantesRepository.GetEstantesDeLaGondola(gondola.Id).Result.ToList();


                    if (gondola.Imagen != null)
                    {
                        gondola.Image = gondola.Id + "_" + gondola.Imagen.ToString("yyyyMMddHHmmss") + ".jpg";
                    }

                    GetEstantes(gondola);

                    if (gondola.IdCategoria.HasValue)
                        gondola.Categoria = categoriasDeArticuloRepository.Get(gondola.IdCategoria.Value).Result;

                }
                pagedResult.PagedList = pagedList;
                var totalPages = Math.Ceiling((double)(pagedResult.TotalRecords / (double)(pageSize ?? 25)));
                pagedResult.TotalPages = totalPages == 0 ? 1 : totalPages;
            });

            t.Wait();

            return Task.FromResult(pagedResult);


        }

        public override async Task<Gondola> Get(long id)
        {
            var gondola = new Gondola();
            var t = Task.Run(() =>
            {
                gondola = base.Get(id).Result;
                if (gondola != null && gondola.Id > 0)
                {
                    GetEstantes(gondola);

                    foreach (var estante in gondola.Estantes)
                    {
                        estante.Articulos = articulosPorEstanteRepository.GetArticulosPorEstante(estante.Id).Result.ToList();
                        foreach (var articulo in estante.Articulos)
                        {
                            articulo.Decoraciones = articulosDecoracionesRepository.GetFromArticulo(articulo.Id, estante.Id).Result.ToList();


                            articulo.CodigosDeBarra = codigosDeBarraRepo.GetCodigosDelArticulo(articulo.IdArticulo).Result.ToList();

                        }
                    }

                    if (gondola.IdCategoria.HasValue)
                        gondola.Categoria = categoriasDeArticuloRepository.Get(gondola.IdCategoria.Value).Result;
                    gondola.Grilla = grillasRepository.GetFromGondola(id);
                }


            });
            t.Wait();

            return gondola;
        }

        public async Task<ServiceResult> UpdateGondolaConEstantes(Gondola gondola)
        {
            var result = new ServiceResult();
            try
            {
                var t = Task.Run(() =>
                {
                    if (gondola.Estantes.Any())
                    {
                        var estantes = estantesRepository.Get(new string[] { "Articulos", "Decoraciones" }).Result.TakeWhile(x => gondola.Estantes.Select(z => z.Id).Contains(x.Id));
                        if (estantes != null && estantes.Any())
                            throw new Exception("Ya existen estantes en esta góndola.");

                        foreach (var estante in gondola.Estantes)
                        {
                            if (estante.Articulos.Any())
                            {
                                var articulos = articulosRepository.GetPorIds(estante.Articulos.Select(x => x.IdArticulo).ToList()).Result;
                                if (!articulos.Any())
                                    throw new Exception("No se han encontrado los artículos solicitados.");

                                if (articulos.Count() < estante.Articulos.Select(x => x.Id).Distinct().Count())
                                    throw new Exception("No todos los artículos que desea insertar existen.");
                            }
                        }
                    }
                });

                t.Wait();

                if (!result.HasErrors)
                {
                    gondola.Creado = DateTime.Now;
                    var updateResult = await gondolasRepository.UpdateGondolaEstantesArticulos(gondola, new string[] { "Estantes", "Articulos", "Grilla", "Imagen", "Image", "Categoria" });
                    result = GetServiceResult(ServiceMethod.Update, "Góndola", updateResult);
                }
            }
            catch (Exception ex)
            {
                result.HasErrors = true;
                result.Message = "Gondola - Estantes - Articulos";
                result.StatusCode = ServiceMethodsStatusCode.Error;
                result.Notes = ex.Message;
                result.Method = "Update";
            }

            return result;
        }

        private void GetEstantes(Gondola gondola)
        {
            gondola.Estantes = estantesRepository.GetEstantesDeLaGondola(gondola.Id).Result.ToList();
            foreach (var estante in gondola.Estantes)
            {
                estante.Articulos = articulosPorEstanteRepository.GetArticulosPorEstante(estante.Id).Result.ToList();
                estante.Decoraciones = decoracionesEstantesRepository.GetDecoracionesDeEstante(estante.Id).Result;

            }

            foreach (var estante in gondola.Estantes)
            {
                foreach (var articulo in estante.Articulos)
                {
                    articulo.Decoraciones = articulosDecoracionesRepository.GetFromArticulo(articulo.Id, estante.Id).Result.ToList();
                }

            }

        }

        private async Task<bool> ValidarExisteArchivo(long idArchivo)
        {
            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas", };
            return (await archivosRepository.Get(idArchivo, columnsToIgnore)).Id > 0;
        }

        public async Task<long> GetCurentCount()
        {
            return (await gondolasRepository.GetCurentCount());
        }
    }
}
