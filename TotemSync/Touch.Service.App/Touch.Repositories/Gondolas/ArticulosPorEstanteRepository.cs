using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class ArticulosPorEstanteRepository : SingleEntityComunRepository<ArticuloEstante>, IArticulosPorEstanteRepository
    {
        private readonly IArticulosDestacadosRepository articulosDestacadosRepository;
        private readonly IArticulosDecoracionesRepository articulosDecoracionesRepository;
        private readonly IArchivosRepository archivosRepository;
        private readonly IArticulosRepository articulosRepository;

        public ArticulosPorEstanteRepository(IConfiguration configuration,
            IArticulosDestacadosRepository articulosDestacadosRepository,
            IArticulosDecoracionesRepository articulosDecoracionesRepository,
            IArchivosRepository archivosRepository,
            IArticulosRepository articulosRepository) : base(configuration)
        {
            this.articulosDecoracionesRepository = articulosDecoracionesRepository;
            this.articulosDestacadosRepository = articulosDestacadosRepository;
            this.archivosRepository = archivosRepository;
            this.articulosRepository = articulosRepository;
        }

        public async Task<bool> DeleteAllArticulosDelEstante(long idEstante)
        {
            var result = false;
            using SqlTransaction tran = OpenConnectionWithTransaction().Result;
            try
            {
                var t = Task.Run(() =>
                {
                    result = DeleteAllArticulosDelEstante(idEstante, tran).Result;
                });
                t.Wait();
                tran.Commit();
            }
            catch (Exception)
            {
                tran.Rollback();
            }
            return result;
        }

        public async Task<bool> DeleteAllArticulosDelEstante(long idEstante, SqlTransaction tran)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_estante = @id_estante";
            Parameters = new Dictionary<string, object>()
            {
                { "id_estante", idEstante},
                { "modificado", DateTime.Now}
            };

            var result = await ExecuteInsertOrUpdate(Sql, Parameters, tran);
            if (!result)
                throw new Exception("No se puede eliminar alguno de los artículos del estante");

            //obtiene los ids de las relaciones del articulo y el estante.
            var articulosEstante = GetArticulosPorEstante(idEstante, tran).Result.ToList();
            var ids = articulosEstante.Select(x => x.Id);
            foreach (var id in ids)
            {
                result = articulosDecoracionesRepository.DeleteFromIdArticuloEstante(id, tran).Result;
                if (!result)
                    throw new Exception("No se puede eliminar alguna de las decoraciones de artículos del estante");

                result = articulosDestacadosRepository.DeleteFromIdArticuloEstante(id, tran).Result;
                if (!result)
                    throw new Exception("No se puede eliminar alguna de los de artículos destacados del estante");
            }
            return result;
        }

        public async Task<IEnumerable<ArticuloEstante>> GetArticulosPorEstante(long id, SqlTransaction tran = null)
        {
            var sql = "Select ar.id, a.id as idArticulo, a.nombre, ar.id_estante as IdEstante, ar.origen_x as OrigenX, ar.origen_y as OrigenY, " +
                "ar.alto, ar.ancho, ar.cantidad_x as CantidadX, ar.cantidad_y as CantidadY, ar.eliminado, ar.estilo_precio as EstiloPrecio, " +
                "ar.color_estilo_precio_fondo  as ColorEstiloPrecioFondo     , ar.color_estilo_precio_frente  as ColorEstiloPrecioFrente    , ar.mostrar_precio   as MostrarPrecio  " +
                " , ar.mostrar_nombre as MostrarNombre " +
               From + ", articulo a " +
               Where + " and a.id = ar.id_articulo and ar.id_estante = @id_estante and ar.eliminado = 0";
            var parameters = new Dictionary<string, object>()
            {
                { "id_estante", id },
            };

            if (tran == null)
                return await GetListOf<ArticuloEstante>(sql, parameters);
            else
                return await GetListOf<ArticuloEstante>(sql, tran, parameters);
        }

        public override Task<bool> Delete(ArticuloEstante entity, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado " +
                "WHERE eliminado = 0 and id_estante = @id_estante and id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>();
            GetParameter(Parameters, entity, "idEstante");
            GetParameter(Parameters, entity, "idArticulo");
            GetParameter(Parameters, entity, "modificado");
            return ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<bool> Insert(ArticuloEstante entity, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }


            bool result = false;
            using SqlTransaction tran = OpenConnectionWithTransaction().Result;
            foreach (var item in entity.Decoraciones)
            {


                var archivo = archivosRepository.Get(item.IdArchivo, new string[] { "File", "Tipo", "Miniaturas" }).Result;

                try
                {
                    columnsToIgnore = columnsToIgnore.Append("Decoraciones").ToArray();
                    columnsToIgnore = columnsToIgnore.Append("EsDestacado").ToArray();
                    var idArticuloEstante = InsertAndGetId(entity, tran, columnsToIgnore).Result;

                    if (idArticuloEstante > 0)
                    {
                        if (entity.Decoraciones != null)
                        {
                            item.IdArticuloEstante = idArticuloEstante;
                            item.Creado = entity.Creado;

                            if (item.IdArchivo == 0)
                                columnsToIgnore = columnsToIgnore.Append("IdArchivo").ToArray();

                            if (entity.Decoraciones != null)
                            {
                                if (archivo == null || archivo.Id == 0 || item.IdArchivo == 0)
                                    columnsToIgnore = columnsToIgnore.Append("IdArchivo").ToArray();
                            }

                            columnsToIgnore = columnsToIgnore.Append("Destacado").ToArray();
                            result = articulosDecoracionesRepository.Insert(item, tran, columnsToIgnore).Result;
                            if (!result)
                                throw new Exception("Error al insertar las decoraciones del artículo");

                            if (entity.EsDestacado())
                            {
                                item.Destacado.IdArticuloEstante = idArticuloEstante;
                                item.Destacado.Creado = entity.Creado;

                                result = articulosDestacadosRepository.Insert(item.Destacado, tran).Result;
                                if (!result)
                                    throw new Exception("Error al insertar el artículo destacado");
                            }
                        }
                    }


                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result = false;
                }


            }
            if (result)
                tran.Commit();

            return result;
        }

        public Task<bool> Insert(List<ArticuloEstante> articulos, long idEstante, DateTime dateTime, SqlTransaction tran)
        {
            var result = true;
            try
            {
                foreach (var articulo in articulos)
                {
                    if (!ValidarExisteArticulo(articulo.IdArticulo))
                        continue;

                    articulo.IdEstante = idEstante;
                    articulo.Creado = dateTime;

                    var idArticuloPorEstante = InsertAndGetId(articulo, tran, new string[] { "Decoraciones", "EsDestacado", "CodigosDeBarra" }).Result;
                    if (idArticuloPorEstante <= 0)
                        throw new Exception("No se pudo insertar el articulo en el estante");

                    if (articulo.Decoraciones.Count > 0)
                        result = InsertarDecoracionesDelArticuloEstante(dateTime, tran, articulo, idArticuloPorEstante);
                    if (!result)
                        throw new Exception("No se pudo insertar la decoración del articulo en el estante");


                    //result = InsertarDestacadosDelArticuloEstante(dateTime, tran, articulo, idArticuloPorEstante);
                    //if (!result)
                    //    throw new Exception("No se pudo destacar el artículo en el estante");
                }
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Task.FromResult(result);
        }

        private bool ValidarExisteArticulo(long idArticulo)
        {
            return articulosRepository.Get(idArticulo).Id > 0;
        }

        private bool InsertarDestacadosDelArticuloEstante(DateTime dateTime, SqlTransaction tran, ArticuloEstante articulo, long idArticuloPorEstante)
        {
            bool result = false;
            foreach (var item in articulo.Decoraciones)
            {
                item.Destacado.Creado = dateTime;
                item.Destacado.IdArticuloEstante = idArticuloPorEstante;
                result = articulosDestacadosRepository.Insert(item.Destacado, tran).Result;
                if (!result)
                    throw new Exception("Error al destacar el artículo.");

            }

            return result;
        }

        private bool InsertarDecoracionesDelArticuloEstante(DateTime dateTime, SqlTransaction tran, ArticuloEstante articulo, long idArticuloPorEstante)
        {
            bool result = false;

            foreach (var item in articulo.Decoraciones)
            {
                item.IdArticuloEstante = idArticuloPorEstante;
                item.Creado = dateTime;

                var columnsToIgnoreDecoracion = new string[] { "Destacado" };

                if (item.IdArchivo == 0)
                    columnsToIgnoreDecoracion = columnsToIgnoreDecoracion.Append("IdArchivo").ToArray();

                result = articulosDecoracionesRepository.Insert(item, tran, columnsToIgnoreDecoracion).Result;
                if (!result)
                    throw new Exception("Error al insertar decoración del artículo.");
            }
            return result;
        }
    }
}
