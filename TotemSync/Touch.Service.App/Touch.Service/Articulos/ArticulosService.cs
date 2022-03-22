using PagedList;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;
using Touch.Repositories.Publicaciones;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Articulos
{
    public class ArticulosService : BaseService, IArticulosService
    {
        private readonly IArticulosRepository articulosRepository;
        private readonly IArticuloMultipleRepository articuloMultipleRepository;
        private readonly IFabricantesRepository fabricantesRepository;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;
        private readonly ITipoDeArticuloRepository tipoDeArticuloRepository;
        private readonly IAtributosService atributosService;
        private readonly ICategoriaAsociadaAlArticuloRepository categoriaArticuloRepo;

        private readonly IBusquedaDeArticulosService busquedaDeArticulosService;
        private readonly IArchivosService archivosService;
        private readonly ISingleEntityComunRepository<TipoArchivo> tiposArchivoService;
        private readonly ISponsoreadosService sponsoreadosService;
        private readonly ICodigosDeBarraRepository codigosDeBarraRepo;
        private readonly IPublicacionesRepository publicacionesRepository;
        private readonly ITipoObjetoPublicitarRepository tipoObjetoPublicitarRepository;
        private readonly IObjetoAPublicitarRepository objetoAPublicitarRepository;
        private readonly IGondolasRepository gondolasRepository;

        public ArticulosService(IArticulosRepository articulosRepository, IFabricantesRepository fabricantesRepository,
            IAtributosService atributosService, ICategoriasDeArticuloRepository categoriasDeArticuloRepository,
            ITipoDeArticuloRepository tipoDeArticuloRepository, IArticuloMultipleRepository articuloMultipleRepository,
            IBusquedaDeArticulosService busquedaDeArticulosService,
            IArchivosService archivosService, ISingleEntityComunRepository<TipoArchivo> tiposArchivoService,
            ISponsoreadosService sponsoreadosService, ICodigosDeBarraRepository codigosRepo,
            ICategoriaAsociadaAlArticuloRepository categoriaArticuloRepo,
            IPublicacionesRepository publicacionesRepository,
            ITipoObjetoPublicitarRepository tipoObjetoPublicitarRepository,
            IObjetoAPublicitarRepository objetoAPublicitarRepository,
            IGondolasRepository gondolasRepository)
        {
            this.articulosRepository = articulosRepository;
            this.fabricantesRepository = fabricantesRepository;
            this.atributosService = atributosService;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
            this.tipoDeArticuloRepository = tipoDeArticuloRepository;
            this.articuloMultipleRepository = articuloMultipleRepository;

            this.busquedaDeArticulosService = busquedaDeArticulosService;
            this.archivosService = archivosService;
            this.tiposArchivoService = tiposArchivoService;
            this.sponsoreadosService = sponsoreadosService;
            codigosDeBarraRepo = codigosRepo;
            this.categoriaArticuloRepo = categoriaArticuloRepo;
            this.publicacionesRepository = publicacionesRepository;
            this.tipoObjetoPublicitarRepository = tipoObjetoPublicitarRepository;
            this.objetoAPublicitarRepository = objetoAPublicitarRepository;
            this.gondolasRepository = gondolasRepository;
        }

        #region Metodos DELETE
        public async Task<ServiceResult> DeleteAlternativo(ArticuloMultiple articulo)
        {
            try
            {
                articuloMultipleRepository.SetNombreTabla("articulos_alternativos");
                return GetServiceResult(ServiceMethod.Delete, "Articulos Alternativos", await articuloMultipleRepository.Delete(articulo));
            }
            catch (Exception ex)
            {
                var result = new ServiceResult()
                {
                    Message = ex.Message,
                    HasErrors = true,
                    Method = ServiceMethod.Delete.ToString(),
                    StatusCode = ServiceMethodsStatusCode.Error
                };
                return result;
            }
        }

        public async Task<ServiceResult> DeleteCruzado(ArticuloMultiple articulo)
        {
            try
            {
                articuloMultipleRepository.SetNombreTabla("articulos_cruzados");
                return GetServiceResult(ServiceMethod.Delete, "Articulos Alternativos", await articuloMultipleRepository.Delete(articulo));
            }
            catch (Exception ex)
            {
                var result = new ServiceResult()
                {
                    Message = ex.Message,
                    HasErrors = true,
                    Method = ServiceMethod.Delete.ToString(),
                    StatusCode = ServiceMethodsStatusCode.Error
                };
                return result;
            }
        }

        public async Task<ServiceResult> Delete(long id)
        {
            var existeArticuloEnPubli = false;
            var existeArticuloEnGondola = false;

            var t1 = Task.Run(() =>
            {
                var tipos = tipoObjetoPublicitarRepository.Get("artículo").Result;
                if (tipos.Any())
                {
                    //Verificar si el articulo existe en una publicacion
                    var idTipo = tipos.FirstOrDefault().Id;
                    existeArticuloEnPubli = objetoAPublicitarRepository.GetPorIdObjetoYTipo(id, idTipo, new string[] { "Objeto", "Tipo" }).Result.Any();

                    //Verificar si el articulo existe en una gondola
                    existeArticuloEnGondola = gondolasRepository.GetGondolasDelArticulo(id).Result.Any();
                }

            });
            t1.Wait();

            if (existeArticuloEnPubli || existeArticuloEnGondola)
                return GetServiceResult(ServiceMethod.Delete, "El Artículo esta asignado a una publicación o una góndola", false);

            var articulo = new Articulo() { Id = id };
            if (await articulosRepository.Delete(articulo))
            {
                var t = Task.Run(() =>
                {
                    //eliminar alternativos   
                    articuloMultipleRepository.SetNombreTabla("articulos_alternativos");
                    articuloMultipleRepository.DeletAllFromArticulo(id);

                    //eliminar cruzados
                    articuloMultipleRepository.SetNombreTabla("articulos_cruzados");
                    articuloMultipleRepository.DeletAllFromArticulo(id);

                    //eliminar atributos del articulo
                    atributosService.DeleteAtributosDelArticulo(id);

                    //elimina los codigos de barra asociados al articulo
                    codigosDeBarraRepo.DeleteCodigosDelArticulo(id);

                    //Eliminar articulos publicados
                    var tipo = tipoObjetoPublicitarRepository.GetPorTipo("Articulo").Result;
                    if (tipo.Any())
                    {
                        var idTipoObjetoPublicitar = tipo.FirstOrDefault().Id;
                        if (idTipoObjetoPublicitar > 0)
                            publicacionesRepository.DeleteObjetosPublicados(id, idTipoObjetoPublicitar);
                    }
                });
                t.Wait();

                return GetServiceResult(ServiceMethod.Delete, articulo.GetType().Name, true);
            }
            return GetServiceResult(ServiceMethod.Delete, articulo.GetType().Name, false);
        }

        public async Task<ServiceResult> DeleteAtributosDelArticulo(long id, IEnumerable<long> idsAtributos)
        {
            return await atributosService.DeleteAtributosDelArticulo(id, idsAtributos);
        }

        #endregion

        #region Metodos GET
        public Task<PagedResult> GetRelaciones(int? pageNumber, int? pageSize, string fechaSincro)
        {
            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, 0);

            var t = Task.Run(() =>
            {

                DateTime dateTime = DateTime.ParseExact(fechaSincro, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                var relaciones = articuloMultipleRepository.GetRelaciones(dateTime).Result.ToList().ToList();


                pagedResult.TotalRecords = relaciones.Count();
                var pagedList = new PagedList<Relacion>(relaciones, pageNumber ?? 1, pageSize ?? 25);


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

                var articulos = articulosRepository.GetWithDeleted(dateTime).Result.ToList().ToList();


                pagedResult.TotalRecords = articulos.Count();
                var pagedList = new PagedList<Articulo>(articulos, pageNumber ?? 1, pageSize ?? 25);


                var idsSponsoreados = sponsoreadosService.Get(DateTime.Now).Result.Select(x => x.IdArticulo).ToList();

                var codigosDeBarra = codigosDeBarraRepo.Get().Result;

                foreach (var articulo in pagedList)
                {
                    

                    articulo.Codigos = codigosDeBarra.Where(x => x.IdArticulo == articulo.Id).ToList();
                    articulo.Archivos = archivosService.GetArchivosDelArticulo(articulo.Id).Result.Where(x => x.Size == "Original").ToList();

                    if (gondolasRepository.GetGondolasDelArticulo(articulo.Id).Result.Any())
                        foreach (var archivo in articulo.Archivos)
                        {
                            archivo.Size = "Gondola";
                        }

                }

                foreach (var id in idsSponsoreados)
                    articulos.FirstOrDefault(x => x.Id == id).Sponsoreado = true;


                IncluirDetalles(pagedList.ToList());

                pagedResult.PagedList = pagedList;
                var totalPages = Math.Ceiling((double)(pagedResult.TotalRecords / (double)(pageSize ?? 25)));
                pagedResult.TotalPages = totalPages == 0 ? 1 : totalPages;
            });

            t.Wait();

            return Task.FromResult(pagedResult);




        }

        public async Task<Articulo> GetArticulo(long id, bool incluirDetalles = true)
        {
            var articulo = (Articulo)await articulosRepository.Get(id);
            var t = Task.Run(() =>
            {
                if (incluirDetalles)
                {
                    articulo.Fabricante = (Fabricante)fabricantesRepository.Get(articulo.IdFabricante).Result;

                    articulo.Atributos.AddRange(atributosService.GetAtributosDelArticulo(articulo.Id).Result);

                    articulo.Categorias = (List<CategoriaDeArticulo>)categoriasDeArticuloRepository.GetCategoriasDelArticulo(articulo.Id).Result;

                    articulo.Tipo = (TipoArticulo)tipoDeArticuloRepository.Get(articulo.IdTipo).Result;

                    articulo.Archivos = (archivosService.GetArchivosDelArticulo(articulo.Id)).Result.ToList();

                    articulo.Codigos = (List<CodigoDeBarras>)codigosDeBarraRepo.GetCodigosDelArticulo(articulo.Id).Result;
                }
            });
            t.Wait();

            return articulo;
        }

        public async Task<IEnumerable<Articulo>> GetArticulos(bool incluirDetalles = true)
        {
            var articulos = (IEnumerable<Articulo>)await articulosRepository.Get();

            var idsSponsoreados = (await sponsoreadosService.Get(DateTime.Now)).Select(x => x.IdArticulo).ToList();

            foreach (var id in idsSponsoreados)
                articulos.FirstOrDefault(x => x.Id == id).Sponsoreado = true;

            articulos = articulos.OrderByDescending(i => i.Sponsoreado).ThenBy(x => x.Id).ToList();

            if (incluirDetalles)
                IncluirDetalles(articulos.ToList());

            return articulos;
        }

        public Task<ArticulosPaginados> GetArticulosPaginados(int pageNumber, int pageSize, bool incluirDetalles = true)
        {
            var articulosPaginados = new ArticulosPaginados();

            var t = Task.Run(() =>
            {
                var articulosList = (List<Articulo>)articulosRepository.Get().Result;
                var pagedList = new PagedList<Articulo>(articulosList, pageNumber, pageSize);

                var idsSponsoreados = sponsoreadosService.Get(DateTime.Now).Result.Select(x => x.IdArticulo).ToList();

                articulosList = articulosList.SelectMany(x => pagedList.Where(z => z.Id == x.Id)).OrderByDescending(i => i.Sponsoreado).ThenBy(x => x.Id).ToList();

                var codigosDeBarra = codigosDeBarraRepo.Get().Result;
                foreach (var articulo in articulosList)
                {
                    articulo.Codigos = codigosDeBarra.Where(x => x.IdArticulo == articulo.Id).ToList();
                    articulo.Archivos = archivosService.GetArchivosDelArticulo(articulo.Id).Result.ToList();
                }

                foreach (var id in idsSponsoreados)
                    articulosList.FirstOrDefault(x => x.Id == id).Sponsoreado = true;

                if (incluirDetalles)
                    IncluirDetalles(articulosList.ToList());



                articulosPaginados.Articulos.AddRange(articulosList);
                articulosPaginados.PageNumber = pageNumber;
                articulosPaginados.PageSize = pageSize;
                articulosPaginados.TotalRecords = pagedList.TotalItemCount;
            });
            t.Wait();

            return Task.FromResult(articulosPaginados);
        }

        public Task<ArticulosPaginados> GetArticulosPaginadosFiltrados(int pageNumber, int pageSize, string nombre)
        {
            var articulosPaginados = new ArticulosPaginados();

            var t = Task.Run(() =>
            {
                var articulosList = busquedaDeArticulosService.BuscarArticulosPorAtributos(nombre).Result.ToList();
                var pagedList = new PagedList<Articulo>(articulosList, pageNumber, pageSize);

                var idsSponsoreados = sponsoreadosService.Get(DateTime.Now).Result.Select(x => x.IdArticulo).ToList();

                articulosList = articulosList.SelectMany(x => pagedList.Where(z => z.Id == x.Id)).OrderByDescending(i => i.Sponsoreado).ThenBy(x => x.Id).ToList();

                var codigosDeBarra = codigosDeBarraRepo.Get().Result;
                foreach (var articulo in articulosList)
                {
                    articulo.Codigos = codigosDeBarra.Where(x => x.IdArticulo == articulo.Id).ToList();
                    articulo.Archivos = archivosService.GetArchivosDelArticulo(articulo.Id).Result.ToList();
                }

                foreach (var id in idsSponsoreados)
                    articulosList.FirstOrDefault(x => x.Id == id).Sponsoreado = true;

                articulosPaginados.Articulos.AddRange(articulosList);
                articulosPaginados.PageNumber = pageNumber;
                articulosPaginados.PageSize = pageSize;
                articulosPaginados.TotalRecords = pagedList.TotalItemCount;
            });
            t.Wait();

            return Task.FromResult(articulosPaginados);
        }

        /// <summary>
        /// Obtiene la lista de articulos segun filtros de busqueda
        /// </summary>
        public Task<List<Articulo>> GetArticulos(Articulo articulo)
        {

            var tArticulosPorNombre = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorNombre(articulo));
            var tArticulosPorFabricante = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorFabricante(articulo));
            var tArticulosPorCategoria = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorCategoria(articulo));
            var tArticulosPorSKU = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorSKU(articulo));
            var tArticulosPorTipo = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorTipo(articulo));
            var tArticulosPorAtributos = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorAtributos(articulo));
            var tArticulosPorCodigoDeBarras = Task.Run(() => busquedaDeArticulosService.BuscarArticulosPorCodigoDeBarras(articulo));
            var tArchivos = Task.Run(() => archivosService.GetArchivosDelArticulo(articulo.Id).Result.ToList());
            var tArticulosPorEstadoActivo = Task.Run(() =>
            {
                if (!articulo.Activo)
                    busquedaDeArticulosService.BuscarArticulosPorEstadoActivo(articulo.Activo).Result.ToList();
                return new List<Articulo>();
            });

            Task.WaitAll(tArticulosPorNombre, tArticulosPorFabricante, tArticulosPorCategoria, tArticulosPorSKU,
                tArticulosPorTipo, tArticulosPorAtributos, tArticulosPorCodigoDeBarras, tArticulosPorEstadoActivo, tArchivos);

            var listas = new List<IEnumerable<Articulo>>()
            {
                tArticulosPorNombre.Result,
                tArticulosPorFabricante.Result,
                tArticulosPorCategoria.Result,
                tArticulosPorSKU.Result,
                tArticulosPorTipo.Result,
                tArticulosPorAtributos.Result,
                tArticulosPorCodigoDeBarras.Result,
                tArticulosPorEstadoActivo.Result
            };

            var result = ObtenerArticulosComunes(listas.Where(x => x.Any()).ToArray()).ToList();
            IncluirDetalles(result);

            return Task.FromResult(result);
        }

        public Task<List<Articulo>> GetArticulosAlternativos(long id, bool incluirDetalles = true)
        {
            articuloMultipleRepository.SetNombreTabla("articulos_alternativos");
            var articulos = new List<Articulo>();
            var t = Task.Run(() =>
            {
                var articulosMultiples = articuloMultipleRepository.GetAll(id).Result;
                foreach (var articuloMultiple in ((IEnumerable<ArticuloMultiple>)articulosMultiples).Where(x => x.Id > 0))
                    articulos.Add(GetArticulo(articuloMultiple.IdDestino).Result);
            });
            t.Wait();

            if (incluirDetalles)
                IncluirDetalles(articulos);

            return Task.FromResult(articulos);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosCruzados(long id, bool incluirDetalles = true)
        {
            articuloMultipleRepository.SetNombreTabla("articulos_cruzados");
            var articulosMultiples = await articuloMultipleRepository.GetAll(id);

            var articulos = new List<Articulo>();
            foreach (var articuloMultiple in (IEnumerable<ArticuloMultiple>)articulosMultiples)
                articulos.Add(await GetArticulo(articuloMultiple.IdDestino));

            if (incluirDetalles)
                IncluirDetalles(articulos);

            return articulos;
        }

        public async Task<IEnumerable<Atributo>> GetAtributosAlArticulo(long id)
        {
            return await atributosService.GetAtributosDelArticulo(id, false);
        }

        public async Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id)
        {
            return await archivosService.GetArchivosDelArticulo(id);
        }

        #endregion

        #region Metodos INSERT y UPDATE

        public async Task<ServiceResult> InsertarAlternativos(IEnumerable<ArticuloMultiple> articulos)
        {
            try
            {
                var result = new ServiceResult
                {
                    StatusCode = ServiceMethodsStatusCode.Ok,
                    Message = "Se ha insertado la relación correctamente",
                    HasErrors = false,
                    Method = "Insertar Alternativos"
                };

                articuloMultipleRepository.SetNombreTabla("articulos_alternativos");

                //TODO: HACER una query sobre articulos que devuelva los articulos con "id" In articulos.select(x=>x.id).tolist() para no traer todos los articulos
                // ya que solo se necesita saber si dichos articulos existen.

                var articulosExistentes = await articulosRepository.Get();
                foreach (var art in articulos)
                {
                    VerificarSiExistenLosArticulosParaInsertar(result, articulosExistentes, art);
                    if (result.HasErrors)
                    {
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        result.HasErrors = true;
                        result.Message = "Alguno de los articulos alternativos ya existe.";

                        continue;
                    };

                    var alternativosExistentes = await articuloMultipleRepository.GetAll(art.IdOrigen);

                    //validar que no exista el par alternativo. Si existe lo descarta
                    if (alternativosExistentes.Any(x => ((ArticuloMultiple)x).IdOrigen == art.IdOrigen && ((ArticuloMultiple)x).IdDestino == art.IdDestino))
                    {
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        result.HasErrors = true;
                        result.Message = "Alguno de los articulos alternativos ya existe.";

                        continue;
                    }

                    if (!await articuloMultipleRepository.Insert(art))
                        return GetServiceResult(ServiceMethod.Insert, "Articulos Alternativos", false);
                }

                return result;
            }
            catch (Exception ex)
            {
                return ObtenerResultadoDeError(ex, ServiceMethod.Insert);
            }
        }

        public async Task<ServiceResult> InsertarCruzados(IEnumerable<ArticuloMultiple> articulos)
        {
            try
            {
                var result = new ServiceResult
                {
                    StatusCode = ServiceMethodsStatusCode.Ok,
                    Message = "Se ha insertado la relación correctamente",
                    HasErrors = false,
                    Method = "Insertar Alternativos"
                };

                articuloMultipleRepository.SetNombreTabla("articulos_cruzados");

                var articulosExistentes = await articulosRepository.Get();

                foreach (var art in articulos)
                {
                    VerificarSiExistenLosArticulosParaInsertar(result, articulosExistentes, art);
                    if (result.HasErrors)
                    {
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        result.HasErrors = true;
                        result.Message = "Alguno de los articulos alternativos ya existe.";

                        continue;
                    };

                    var cruzadosExistentes = await articuloMultipleRepository.GetAll(art.IdOrigen);

                    //validar que no exista el par cruzado. Si existe lo descarta
                    if (cruzadosExistentes.Any(x => ((ArticuloMultiple)x).IdOrigen == art.IdOrigen && ((ArticuloMultiple)x).IdDestino == art.IdDestino))
                    {
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        result.HasErrors = true;
                        result.Message = "Alguno de los articulos alternativos ya existe.";

                        continue;
                    }

                    if (!await articuloMultipleRepository.Insert(art))
                        return GetServiceResult(ServiceMethod.Insert, "Articulos Alternativos", false);
                }

                return result;
            }
            catch (Exception ex)
            {
                return ObtenerResultadoDeError(ex, ServiceMethod.Insert);
            }
        }

        public async Task<ServiceResult> AsociarAtributosAlArticulo(long id, IEnumerable<long> idsAtributos)
        {
            var art = await articulosRepository.Get(id);
            if (art != null && art.Id > 0)
                return await atributosService.InsertarAtributosDelArticulo(id, idsAtributos);
            else
                return GetServiceResult(ServiceMethod.Update, "Atributos del artículo", false);
        }

        public async Task<ServiceResult> Insert(Articulo articulo)
        {
            var result = new ServiceResult()
            {
                HasErrors = false,
                Message = "Se ha insertado el artículo correctamente",
                Method = "Insert",
                StatusCode = ServiceMethodsStatusCode.Ok
            };

            var resultadoDeValidaciones = ObtenerResultadoDeValidaciones(articulo);
            if (resultadoDeValidaciones.HasErrors)
                return resultadoDeValidaciones;

            var idArticulo = await articulosRepository.InsertAndGetId(articulo);
            if (idArticulo <= 0)
                return GetServiceResult(ServiceMethod.Insert, articulo.GetType().Name, false);

            result.IdObjeto = idArticulo;
            articulo.Id = idArticulo;
            //si se insertó el articulo correctamente
            //insertamos codigos de barra, archivos y categorias

            await InsertarCategoriaAsociadaAlArticulo(articulo, result);

            InsertarCodigosDeBarraDelArticulo(articulo, result);

            await InsertarAtributosAlArticulo(articulo, result);

            return result;
        }

        public async Task<ServiceResult> Update(Articulo articulo)
        {
            var tArticulo = Task.Run(() => articulosRepository.Get(articulo.Id));
            var tFabricante = Task.Run(() => fabricantesRepository.Get(articulo.IdFabricante));
            var tTipoArticulo = Task.Run(() => tipoDeArticuloRepository.Get(articulo.IdTipo));

            var existente = tArticulo.Result;
            if (existente == null || existente.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, articulo.GetType().Name);

            var fabricante = tFabricante.Result;
            if (fabricante == null || fabricante.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, fabricante.GetType().Name);

            var tipo = tTipoArticulo.Result;
            if (tipo == null || tipo.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, tipo.GetType().Name);

            Task.WaitAll(tArticulo, tFabricante, tTipoArticulo);
            var resultadoDeUpdate = await articulosRepository.Update(articulo);

            if (resultadoDeUpdate)
            {
                var tActualizarCategoria = Task.Run(() => ActualizarCategoriasDeArticulo(articulo, new ServiceResult()));
                var tActualizarCodigos = Task.Run(() => ActualizarCodigosDeBarraDeArticulo(articulo, new ServiceResult()));
                var tActualizarAtributos = Task.Run(() => ActualizarAtributosDeArticulo(articulo, new ServiceResult()));
                Task.WaitAll(tActualizarCategoria, tActualizarCodigos, tActualizarAtributos);
            }

            return GetServiceResult(ServiceMethod.Update, articulo.GetType().Name, resultadoDeUpdate);
        }

        #endregion


        #region Metodos privados
        private void IncluirDetalles(List<Articulo> articulos)
        {
            var tFabricantes = Task.Run(() => fabricantesRepository.Get());
            var fabricantes = tFabricantes.Result;

            var tTipoArticulo = Task.Run(() => tipoDeArticuloRepository.Get());
            var tiposArticulos = tTipoArticulo.Result;

            foreach (var articulo in articulos)
            {
                var tCategorias = Task.Run(() => categoriasDeArticuloRepository.GetCategoriasDelArticulo(articulo.Id));
                var tAtributos = Task.Run(() => atributosService.GetAtributosDelArticulo(articulo.Id));
                //var tArchivos = Task.Run(() => archivosService.GetArchivosDelArticulo(articulo.Id));
                Task.WaitAll(tFabricantes, tTipoArticulo, tCategorias, tAtributos);

                articulo.Fabricante = (Fabricante)fabricantes.FirstOrDefault(x => x.Id == articulo.IdFabricante);
                articulo.Tipo = (TipoArticulo)tiposArticulos.FirstOrDefault(x => x.Id == articulo.IdTipo);

                articulo.Categorias = (List<CategoriaDeArticulo>)tCategorias.Result;
                articulo.Atributos.AddRange(tAtributos.Result);
                //articulo.Archivos = tArchivos.Result.ToList();
            }
        }

        private IEnumerable<Articulo> ObtenerArticulosComunes(IEnumerable<Articulo>[] listas)
        {
            var result = new List<Articulo>();
            try
            {
                var primerLista = listas.First();

                if (listas.Count() == 1)
                    return primerLista.Distinct();

                //Recorre los atributos de la primera lista
                for (int i = 0; i < primerLista.Count(); i++)
                {
                    //recorre cada una de las siguientes listas para ver si encuentra el atributo pivot
                    for (int j = 1; j < listas.Count(); j++)
                    {
                        //recorre los atributos de la lista a comparar con la primera
                        foreach (var articulo in listas[j])
                        {
                            if (primerLista.Where(x => x.Id == articulo.Id).Any())
                                result.Add(articulo);
                        }
                    }
                }

                return result.Distinct();
            }
            catch (Exception)
            {
                return result;
            }
        }

        private ServiceResult ObtenerErrorDeElementosIguales(string articulos, ServiceMethod method)
        {
            return new ServiceResult()
            {
                Method = method.ToString(),
                StatusCode = ServiceMethodsStatusCode.Error,
                HasErrors = true,
                Message = "Los articulos " + articulos + " son iguales."
            };
        }

        private void VerificarSiExistenLosArticulosParaInsertar(ServiceResult result, IEnumerable<ComunEntity> articulosExistentes, ArticuloMultiple art)
        {
            var art1 = articulosExistentes.FirstOrDefault(x => x.Id == art.IdOrigen);
            var art2 = articulosExistentes.FirstOrDefault(x => x.Id == art.IdDestino);

            if (art1 == null || art2 == null)
            {
                result.HasErrors = true;
                result.Message = "Alguno de los articulos no existe";
                result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                return;
            }

            if (art1 == art2)
                result = ObtenerErrorDeElementosIguales("alternativos o cruzados", ServiceMethod.Insert);
        }

        private async Task InsertarCategoriaAsociadaAlArticulo(Articulo articulo, ServiceResult result)
        {
            var categoriasExistentes = await categoriasDeArticuloRepository.Get(new string[] { "Subcategorias" });
            var ids = articulo.Categorias.Select(z => z.Id).ToList();
            var categorias = categoriasExistentes.Where(p => ids.Contains(p.Id));

            if (!categorias.Any())
            {
                result.HasErrors = true;
                result.Message = "El artículo se insertó pero, la categoria no se pudo insertar";
                result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                return;
            }

            foreach (var categoria in categorias)
            {
                //insertar categoria            
                if (categoria.Id > 0)
                {
                    var resultadoDeCategoria = await categoriaArticuloRepo.Insert(new CategoriaAsociadaAlArticulo()
                    { IdArticulo = articulo.Id, IdCategoria = categoria.Id, Creado = DateTime.Now });

                    if (!resultadoDeCategoria)
                    {
                        result.HasErrors = true;
                        result.Message = "El artículo se insertó pero, la categoria no se pudo insertar";
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    }
                }
            }
        }

        private void InsertarCodigosDeBarraDelArticulo(Articulo articulo, ServiceResult result)
        {
            //insertar codigos de barra
            if (articulo.Codigos.Any())
            {
                var t = Task.Run(() =>
                {
                    foreach (var codigo in articulo.Codigos)
                    {
                        codigo.IdArticulo = articulo.Id;
                        var resultadoDeCodigosDeBara = codigosDeBarraRepo.Insert(codigo).Result;

                        if (!resultadoDeCodigosDeBara)
                        {
                            result.HasErrors = true;
                            result.Message = "El artículo se insertó pero, algún código de barras no se pudo insertar";
                            result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        }
                    }
                });
                t.Wait();
            }
        }

        private async Task InsertarAtributosAlArticulo(Articulo articulo, ServiceResult result)
        {
            //insertar codigos de barra
            if (articulo.Atributos.Any())
            {
                var atributosDelArticulo = await atributosService.GetAtributosDelArticulo(articulo.Id);
                var ids = new List<long>();
                foreach (var atributo in articulo.Atributos)
                {
                    if (!atributosDelArticulo.Any(x => x.Id == atributo.Id))
                        ids.Add(atributo.Id);
                }

                var resultadoDeArticulos = await atributosService.InsertarAtributosDelArticulo(articulo.Id, ids);

                if (resultadoDeArticulos.HasErrors)
                    result = resultadoDeArticulos;
            }
        }

        private async Task ActualizarAtributosDeArticulo(Articulo articulo, ServiceResult result)
        {
            var atributosDelArticulo = atributosService.GetAtributosDelArticulo(articulo.Id).Result.ToList();

            // 1- eliminar de la lista de atrubutos ingresados, los que no existen en la bd
            var atributosEliminar = atributosDelArticulo.Where(x => !articulo.Atributos.Select(z => z.Id).Contains(x.Id));

            if (atributosEliminar.Any())
            {
                await atributosService.DeleteAtributosDelArticulo(articulo.Id, atributosEliminar.Select(x => x.Id));

                for (int i = 0; i < atributosDelArticulo.Count(); i++)
                {
                    if (atributosEliminar.Select(x => x.Id).Contains(atributosDelArticulo[i].Id))
                    {
                        atributosDelArticulo.Remove(atributosDelArticulo[i]);
                        i--;
                    }
                }
            }

            // 2- insertar los nuevos atributos
            var atributosExistenes = atributosService.Get().Result;
            var idsAtributosInsertar = atributosExistenes.Select(x => x.Id).Where(z => !atributosDelArticulo.Select(x => x.Id).Contains(z))
                .Where(y => articulo.Atributos.Select(t => t.Id).Contains(y));

            //Si no existen los atributos ingresados
            if (!idsAtributosInsertar.Any())
            {
                result.HasErrors = true;
                result.Message = "El artículo se insertó pero, la categoria no se pudo insertar. No existen las categorias seleccionadas.";
                result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                return;
            }

            //insertar atributo     
            result = atributosService.InsertarAtributosDelArticulo(articulo.Id, idsAtributosInsertar).Result;
        }

        private void ActualizarCodigosDeBarraDeArticulo(Articulo articulo, ServiceResult result)
        {
            var codigosDelArticulo = codigosDeBarraRepo.GetCodigosDelArticulo(articulo.Id).Result;
            var codigos = codigosDelArticulo.Select(z => z.EAN).ToList();

            //Eliminar los codigos que NO estan en la lista enviada por el usuario y SI existen en bd --> eliminar sobrantes
            var codigosEliminar = codigosDelArticulo.Where(p => !codigos.Contains(p.EAN));
            foreach (var codigo in codigosEliminar)
            {
                codigo.Modificado = DateTime.Now;
                codigosDeBarraRepo.Delete(codigo);
            }

            var codigosInsertar = articulo.Codigos.Where(p => !codigos.Contains(p.EAN));
            foreach (var codigo in codigosInsertar.Where(x => !string.IsNullOrWhiteSpace(x.EAN)))
            {
                codigo.IdArticulo = articulo.Id;
                var resultadoDeCodigosDeBara = codigosDeBarraRepo.Insert(codigo).Result;

                if (!resultadoDeCodigosDeBara)
                {
                    result.HasErrors = true;
                    result.Message = "El artículo se actualizo pero algún código de barras no se pudo insertar";
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                }
            }
        }

        private async Task ActualizarCategoriasDeArticulo(Articulo articulo, ServiceResult result)
        {

            if (!articulo.Categorias.Any())
            {
                result.HasErrors = categoriaArticuloRepo.DeleteCategoriasDelArticulo(articulo.Id);
                return;
            }

            var categoriasDelArticulo = categoriaArticuloRepo.GetCategoriasDelArticulo(articulo.Id).ToList();

            // 1- eliminar de la lista de categorias ingresadas, los que no existen en la bd
            var categoriasEliminar = categoriasDelArticulo.Where(x => !articulo.Categorias.Select(z => z.Id).Contains(x.Id));

            if (categoriasEliminar.Any())
            {
                await categoriaArticuloRepo.DeleteCategoriasDelArticulo(articulo.Id, categoriasEliminar.Select(x => x.Id));

                for (int i = 0; i < categoriasDelArticulo.Count(); i++)
                {
                    if (categoriasEliminar.Select(x => x.Id).Contains(categoriasDelArticulo[i].Id))
                    {
                        categoriasDelArticulo.Remove(categoriasDelArticulo[i]);
                        i--;
                    }
                }
            }

            // 2- insertar los nuevos Categorias
            var categoriasExistenes = categoriasDeArticuloRepository.Get().Result;
            var idsCategoriasInsertar = categoriasExistenes.Select(x => x.Id).Where(z => !categoriasDelArticulo.Select(x => x.Id).Contains(z))
                .Where(y => articulo.Categorias.Select(t => t.Id).Contains(y));

            //Si no existen los Categorias ingresados
            if (!idsCategoriasInsertar.Any())
            {
                result.HasErrors = true;
                result.Message = "El artículo se actualizó pero, las categorias no se pudieron actualizad. No existen las categorias seleccionadas.";
                result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                return;
            }

            //insertar Categoria    
            foreach (var idCategoria in idsCategoriasInsertar)
            {
                result.HasErrors = !categoriaArticuloRepo.Insert(new CategoriaAsociadaAlArticulo() { IdArticulo = articulo.Id, IdCategoria = idCategoria, Creado = DateTime.Now }).Result;
                if (result.HasErrors)
                {
                    result.Message = "El artículo se actualizó pero, las categorias no se pudieron actualizad. Hubo un error al actualizar la base de datos.";
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                }
            }
        }

        private ServiceResult ObtenerResultadoDeValidaciones(Articulo articulo)
        {
            if (string.IsNullOrWhiteSpace(articulo.SKU))
                return GetServiceResult(ServiceMethod.Insert, "SKU Missing", false);

            var tArticulosNombre = Task.Run(() => articulosRepository.Get(articulo.Nombre));
            var tArticulosSKU = Task.Run(() => articulosRepository.GetPorSKU(articulo.SKU).Result.FirstOrDefault());
            var tFabticante = Task.Run(() => fabricantesRepository.Get(articulo.IdFabricante));
            var tTipoArticulo = Task.Run(() => tipoDeArticuloRepository.Get(articulo.IdTipo));

            var t = Task.Run(() =>
            {
                atributosService.Get();
                var atributosExistentes = atributosService.Get().Result;

                for (int i = 0; i < articulo.Atributos.Count(); i++)
                {
                    if (!atributosExistentes.Any(x => x.Id == articulo.Atributos[i].Id))
                    {
                        articulo.Atributos.Remove(articulo.Atributos[i]);
                        i--;
                    }
                }
            });
            t.Wait();

            //validar que el nombre del articulo no exista
            var existente = tArticulosNombre.Result
                .Any(x => x.Nombre.ToUpper().Equals(articulo.Nombre) && ((Articulo)x).IdTipo.Equals(articulo.IdTipo));
            if (existente)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, articulo.GetType().Name);

            //validar que el sku no exista
            existente = tArticulosSKU.Result != null && tArticulosSKU.Result.Id > 0;
            if (existente)
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, articulo.GetType().Name);

            //validar que el fabricante exista
            var fabricante = tFabticante.Result;
            if (fabricante == null || fabricante.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, fabricante.GetType().Name);

            //validar que el tipo de articulo exista
            var tipo = tTipoArticulo.Result;
            if (tipo == null || tipo.Id == 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, tipo.GetType().Name);

            Task.WaitAll(tArticulosNombre, tArticulosSKU, tFabticante, tTipoArticulo);
            return new ServiceResult() { HasErrors = false };
        }

        public async Task<long> GetCurentCount()
        {
            return (await articulosRepository.GetCurentCount());
        }

        #endregion
    }
}
