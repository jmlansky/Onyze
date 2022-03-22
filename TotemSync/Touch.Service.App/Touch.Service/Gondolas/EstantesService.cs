using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Gondolas.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Gondolas
{
    public class EstantesService : SingleEntityComunService<Estante>, IEstantesService
    {
        private readonly IGondolasService gondolasService;
        private readonly IEstantesRepository estantesRepository;
        private readonly IArticulosPorEstanteRepository articulosPorEstanteRepository;
        private readonly IArticulosRepository articulosRepository;
        private readonly IEstantesDecoracionesRepository decoracionesEstantesRepository;
        private readonly IArticulosDecoracionesRepository decoracionesArticulosRepository;
        private readonly IArticulosDestacadosRepository articulosDestacadosRepository;
        private readonly IArchivosRepository archivosRepository;


        private string[] columnsToIgnore = { "Articulos", "Decoraciones" };
        public EstantesService(IEstantesRepository estantesRepository, IGondolasService gondolasService,
            IArticulosPorEstanteRepository articulosPorEstanteRepository,
            IArticulosRepository articulosRepository,
            IEstantesDecoracionesRepository decoracionesEstantesRepository,
            IArticulosDecoracionesRepository decoracionesArticulosRepository,
            IArticulosDestacadosRepository articulosDestacadosRepository,
            IArchivosRepository archivosRepository) : base(estantesRepository)
        {
            this.gondolasService = gondolasService;
            this.estantesRepository = estantesRepository;
            this.articulosPorEstanteRepository = articulosPorEstanteRepository;
            this.articulosRepository = articulosRepository;
            this.decoracionesEstantesRepository = decoracionesEstantesRepository;
            this.decoracionesArticulosRepository = decoracionesArticulosRepository;
            this.articulosDestacadosRepository = articulosDestacadosRepository;
            this.archivosRepository = archivosRepository;
        }

        public async Task<IEnumerable<Estante>> GetEstantesDeLaGondola(long id)
        {
            var estantes = new List<Estante>();
            var t = Task.Run(() =>
            {
                estantes = estantesRepository.GetEstantesDeLaGondola(id).Result.ToList();
                GetDecoracionesDeEstantes(estantes);
            });
            t.Wait();
            return estantes;
        }

        public override async Task<IEnumerable<Estante>> Get()
        {
            var estantes = new List<Estante>();
            var t = Task.Run(() =>
            {
                estantes = estantesRepository.Get(columnsToIgnore).Result.ToList();
                GetDecoracionesDeEstantes(estantes);
            });
            t.Wait();
            return estantes;
        }

        public override async Task<Estante> Get(long id)
        {
            var estante = new Estante();
            var t = Task.Run(() =>
            {
                estante = estantesRepository.Get(id, columnsToIgnore).Result;
                GetDecoracionesDeEstantes(new List<Estante>() { estante });
            });
            t.Wait();

            return estante;
        }

        public override async Task<ServiceResult> Insert(Estante estante)
        {
            var result = new ServiceResult()
            {
                HasErrors = false,
                Method = "Insert",
                StatusCode = ServiceMethodsStatusCode.Ok,
                Notes = "Insert Estante - Ok"
            };

            var t = Task.Run(() =>
            {
                try
                {
                    var gondola = gondolasService.Get(estante.IdGondola).Result;
                    if (gondola == null || gondola.Id <= 0)
                        throw new Exception("No existe la góndola");

                    //valida que no exista un estante a la misma altura                    
                    if (gondola.Estantes.Any(x => x.Altura == estante.Altura))
                        throw new Exception("Ya existen estantes a la altura seleccionada para esta góndola.");

                    ValidarArticulosDelEstante(estante);
                    ValidarArchivosDelEstante(estante);

                    var id = estantesRepository.InsertAndGetId(estante, columnsToIgnore).Result;
                    if (id <= 0)
                        throw new Exception("Hubo un error al insertar el estante");

                    result.IdObjeto = id;
                }
                catch (Exception ex)
                {
                    result.HasErrors = true;
                    result.Message = ex.Message;
                    result.StatusCode = ServiceMethodsStatusCode.Error;
                    result.Notes = "Insert Estante - Error";
                }
            });
            t.Wait();

            return result;
        }

        public override async Task<ServiceResult> Update(Estante entity)
        {
            var serviceResult = new ServiceResult { Method = "Update", HasErrors = false, StatusCode = ServiceMethodsStatusCode.Ok };
            var t = Task.Run(() =>
            {
                try
                {
                    var estante = estantesRepository.Get(entity.Id, columnsToIgnore).Result;
                    if (estante == null || estante.Id <= 0)
                        throw new Exception("El estante no existe");

                    var gondola = gondolasService.Get(estante.IdGondola).Result;
                    if (gondola == null || gondola.Id <= 0)
                        throw new Exception("La góndola no existe");

                    ValidarArticulosDelEstante(estante);
                    ValidarArchivosDelEstante(estante);

                    entity.IdGondola = estante.IdGondola;
                    estante.Modificado = DateTime.Now;
                    var result = estantesRepository.Update(entity, columnsToIgnore).Result;

                    if (result)
                        serviceResult.IdObjeto = entity.Id;

                }
                catch (Exception ex)
                {
                    serviceResult.HasErrors = true;
                    serviceResult.Message = ex.Message;
                    serviceResult.StatusCode = ServiceMethodsStatusCode.Error;
                }
            });

            t.Wait();

            return serviceResult;
        }

        public override async Task<ServiceResult> Delete(Estante entity)
        {
            var serviceResult = new ServiceResult { Method = "Delete", HasErrors = false, StatusCode = ServiceMethodsStatusCode.Ok };

            var result = await estantesRepository.Delete(entity);
            serviceResult.HasErrors = result;

            return serviceResult;
        }

        public async Task<ServiceResult> AsociarArticulosAlEstante(IEnumerable<ArticuloEstante> entities)
        {
            var result = GetServiceResult(ServiceMethod.Insert, "Articulo del estante", true);
            var idsArticulosExistentes = (await articulosRepository.Get()).Select(x => x.Id);
            var creado = DateTime.Now;
            var listaInsertar = entities.Where(x => idsArticulosExistentes.Any(z => x.IdArticulo == z));

            if (!listaInsertar.Any())
            {
                result.Message = "Todos los articulos son inexistentes o eliminados.";
                result.HasErrors = true;
                result.StatusCode = ServiceMethodsStatusCode.Error;
                return result;
            }

            if (listaInsertar.Count() < entities.Count())
            {
                result.Message = "Alguno de los articulos son inexistentes o estan eliminados. Puede que no se hayan insertado en el estante";
                result.HasErrors = true;
                result.StatusCode = ServiceMethodsStatusCode.PartialContent;
            }

            var t = Task.Run(() =>
            {
                foreach (var entity in listaInsertar)
                {
                    entity.Creado = creado;
                    result.HasErrors = !articulosPorEstanteRepository.Insert(entity, new string[] { "Nombre", "Decoraciones", "EsDestacado", "Destacado" }).Result;

                    if (result.HasErrors)
                    {
                        result.Message = "Alguno de los articulos puede que no se hayan insertado en el estante";
                        result.HasErrors = true;
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    }
                }
            });
            t.Wait();

            return result;
        }

        public Task<ServiceResult> EliminarArticulosDelEstante(IEnumerable<ArticuloEstante> entities)
        {
            var result = GetServiceResult(ServiceMethod.Delete, "Articulo del estante", true);
            var t = Task.Run(() =>
            {
                var modificado = DateTime.Now;
                foreach (var entity in entities)
                {
                    entity.Modificado = modificado;
                    result.HasErrors = !articulosPorEstanteRepository.Delete(entity, new string[] { "Nombre" }).Result;

                    if (result.HasErrors)
                        result.Message = "Alguno de los articulos puede que no se hayan eliminado del estante";
                }
            });
            t.Wait();

            return Task.FromResult(result);
        }

        public Task<ServiceResult> ActualizarArticulosDelEstante(IEnumerable<ArticuloEstante> entities)
        {
            var result = GetServiceResult(ServiceMethod.Update, "Articulo del estante", true);

            if (!entities.Any())
            {
                result.Message = "No hay articulos para insertar";
                result.HasErrors = true;
                result.StatusCode = ServiceMethodsStatusCode.Error;
            }

            var idEstante = entities.FirstOrDefault().IdEstante;
            var creado = DateTime.Now;

            var t = Task.Run(() =>
            {
                result.HasErrors = !articulosPorEstanteRepository.DeleteAllArticulosDelEstante(idEstante).Result;
                if (result.HasErrors)
                    throw new Exception("Error al eliminar los articulos del estante");

                foreach (var entity in entities)
                {
                    entity.Creado = creado;
                    result.HasErrors = !articulosPorEstanteRepository.Insert(entity, new string[] { "Nombre" }).Result;
                    if (result.HasErrors)
                    {
                        result.Message = "Alguno de los articulos puede que no se hayan actualizado en el estante";
                        result.HasErrors = true;
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    }
                }
            });
            t.Wait();

            return Task.FromResult(result);
        }

        public Task<ServiceResult> EliminarArticulosDelEstante(long id)
        {
            var result = GetServiceResult(ServiceMethod.Update, "Articulo del estante", true);

            var t = Task.Run(() =>
            {
                result.HasErrors = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(id).Result;
            });
            t.Wait();

            return Task.FromResult(result);
        }

        public async Task<IEnumerable<ArticuloEstante>> GetArticulosDelEstante(long id)
        {
            var result = await articulosPorEstanteRepository.GetArticulosPorEstante(id);
            return result;
        }

        private void GetDecoracionesDeEstantes(List<Estante> estantes)
        {
            foreach (var estante in estantes)
            {
                estante.Decoraciones = decoracionesEstantesRepository.GetDecoracionesDeEstante(estante.Id).Result;
                //foreach (var deco in estante.Decoraciones)
                //    deco.Archivo = archivosRepository.Get(deco.IdArchivo).Result.ToList();

                estante.Articulos = articulosPorEstanteRepository.GetArticulosPorEstante(estante.Id).Result.ToList();
                GetDecoracionesDeArticulosDeEstante(estante);
            }
        }

        private void GetDecoracionesDeArticulosDeEstante(Estante estante)
        {
            foreach (var articulo in estante.Articulos)
            {
                articulo.Decoraciones = decoracionesArticulosRepository.GetFromArticulo(articulo.IdArticulo, articulo.IdEstante).Result;
                //articulo.Decoracion.Destacado = articulosDestacadosRepository.GetFromArticulo(articulo.IdArticulo, articulo.IdEstante).Result;
            }
        }

        private void ValidarArticulosDelEstante(Estante estante)
        {
            if (estante.Articulos != null && estante.Articulos.Any())
            {
                var articulosExistentes = articulosRepository.GetPorIds(estante.Articulos.Select(x => x.IdArticulo).Distinct().ToList()).Result;
                if (articulosExistentes.Count() != estante.Articulos.Select(x => x.IdArticulo).Distinct().ToList().Count())
                    throw new Exception("Hay articulos que no existen");

                var hayArticulosEnPosicionesRepetidas = estante.Articulos.GroupBy(s => s.OrigenX).Select(grp => grp.ToList()).ToList().Any(x => x.Count() > 1);
                if (hayArticulosEnPosicionesRepetidas)
                    throw new Exception("Hay artículos en posiciones repetidas");
            }
        }

        private void ValidarArchivosDelEstante(Estante estante)
        {
            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas", };

            if (estante.Decoraciones != null && estante.Decoraciones.Any())
            {
                foreach (var decoracion in estante.Decoraciones)
                {
                    if (decoracion.IdArchivo > 0)
                    {
                        if (decoracion.IdArchivo.HasValue)
                        {
                            var existe = archivosRepository.Get(decoracion.IdArchivo.Value, columnsToIgnore).Id > 0;

                            if (!existe)
                                throw new Exception("No existe el archivo especificado");
                        }
                    }
                }
            }
        }
    }
}
