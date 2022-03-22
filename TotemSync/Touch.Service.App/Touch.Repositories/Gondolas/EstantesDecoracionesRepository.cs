using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class EstantesDecoracionesRepository : SingleEntityComunRepository<EstanteDecoracion>, IEstantesDecoracionesRepository
    {
        private readonly IArticulosPorEstanteRepository articulosPorEstanteRepository;
        private readonly IArchivosRepository archivosRepository;

        public EstantesDecoracionesRepository(IConfiguration configuracion,
            IArticulosPorEstanteRepository articulosPorEstanteRepository,
            IArchivosRepository archivosRepository) : base(configuracion)
        {
            this.articulosPorEstanteRepository = articulosPorEstanteRepository;
        }

        public async Task<bool> DeleteFromEstante(long id, SqlTransaction tran)
        {
            var result = false;
            var t = Task.Run(() =>
            {
                //var idsDecoraciones = GetDecoracionesDeEstante(id).Result.Select(x => x.Id);

                Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_estante = @id";
                Parameters = new Dictionary<string, object>() {
                    { "id", id },
                    { "modificado", DateTime.Now}
                };
                result = ExecuteInsertOrUpdate(Sql, Parameters, tran).Result;
                if (!result)
                    throw new Exception("No se pueden eliminar las decoraciones de los estantes de la góndola.");

                

                //result = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(id, tran).Result;
                //if (!result)
                //    throw new Exception("No se puede eliminar alguno de los artículos del estante");

            });
            t.Wait();

            return result;
        }

        public async Task<List<EstanteDecoracion>> GetDecoracionesDeEstante(long id)
        {
            Sql = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Archivo" }) + " " + From + Where + "and id_estante = @id";
            return GetListOf<EstanteDecoracion>(Sql, new Dictionary<string, object>() { { "id", id } }).Result.ToList();
        }

        public Task<bool> Insert(List<EstanteDecoracion> decoraciones, long id, DateTime date, SqlTransaction tran)
        {
            var result = true;
            try
            {
                var t = Task.Run(() =>
                {
                    foreach (var decoracion in decoraciones)
                    {
                        decoracion.Creado = date;
                        decoracion.IdEstante = id;
                        result = InsertAndGetId(decoracion, tran, new string[] { "Archivos" }).Result > 0;
                        if (!result)
                            throw new Exception("Error al insertar alguna decoración del estante");
                    }
                });
                t.Wait();
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }

            return Task.FromResult(result);
        }

        public override Task<long> InsertAndGetId(EstanteDecoracion entity, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            long id = 0;
            var t = Task.Run(() =>
            {
                if (columnsToIgnore == null)
                    columnsToIgnore = new string[] { "Modificado", "Id" };
                else
                {
                    columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                    columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                }

                if (!entity.IdArchivo.HasValue)
                    columnsToIgnore = columnsToIgnore.Append("IdArchivo").ToArray();

                Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                    " Select SCOPE_IDENTITY()";
                id = Convert.ToInt64(ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran).Result);

                //foreach (var archivo in entity.Archivos)
                //{

                //    if (!ValidarExisteArchivoAsync(archivo.IdArchivo).Result)
                //        continue;

                //    archivo.IdDecoracion = id;
                //    archivo.Creado = entity.Creado;
                //    if (!archivoDecoracionRepository.Insert(archivo, tran).Result)
                //        throw new Exception("No se puede insertar alguno de los archivos de la decoracion");
                //}
            });
            t.Wait();

            return Task.FromResult(id);
        }

        private async Task<bool> ValidarExisteArchivoAsync(long idArchivo)
        {
            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas", };
            return (await archivosRepository.Get(idArchivo, columnsToIgnore)).Id > 0;
        }
    }
}
