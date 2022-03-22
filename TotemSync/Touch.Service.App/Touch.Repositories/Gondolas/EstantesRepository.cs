using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class EstantesRepository : SingleEntityComunRepository<Estante>, IEstantesRepository
    {
        private readonly IEstantesDecoracionesRepository decoracionRepository;
        private readonly IArticulosPorEstanteRepository articulosPorEstanteRepository;

        public EstantesRepository(IConfiguration configuration, IEstantesDecoracionesRepository decoracionRepository,
            IArticulosPorEstanteRepository articulosPorEstanteRepository) : base(configuration)
        {
            this.decoracionRepository = decoracionRepository;
            this.articulosPorEstanteRepository = articulosPorEstanteRepository;
        }

        public override async Task<long> InsertAndGetId(Estante estante, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            long idEstante = 0;
            var t = Task.Run(() =>
            {
                try
                {
                    estante.Creado = DateTime.Now;
                    idEstante = base.InsertAndGetId(estante, tran, columnsToIgnore).Result;
                    if (idEstante > 0)
                    {
                        var result = false;
                        if (estante.Decoraciones.Any())
                        {
                            result = decoracionRepository.Insert(estante.Decoraciones, idEstante, Convert.ToDateTime(estante.Creado), tran).Result;

                            if (!result)
                                throw new Exception("Error al insertar la decoración del estante");
                        }

                        if (estante.Articulos != null && estante.Articulos.Any())
                            result = articulosPorEstanteRepository.Insert(estante.Articulos, idEstante, Convert.ToDateTime(estante.Creado), tran).Result;

                        if (!result)
                            throw new Exception("Error al insertar artículos del estante");
                    }

                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    idEstante = 0;
                }
            });
            t.Wait();
            return idEstante;
        }
        public override async Task<long> InsertAndGetId(Estante estante, string[] columnsToIgnore = null)
        {
            long idEstante = 0;
            var t = Task.Run(() =>
            {
                using SqlTransaction tran = OpenConnectionWithTransaction().Result;
                try
                {
                    estante.Creado = DateTime.Now;
                    idEstante = base.InsertAndGetId(estante, tran, columnsToIgnore).Result;
                    if (idEstante > 0 && estante.Decoraciones.Any())
                    {
                        var result = decoracionRepository.Insert(estante.Decoraciones, idEstante, Convert.ToDateTime(estante.Creado), tran).Result;
                        if (!result)
                            throw new Exception("Error al insertar la decoración del estante");

                        if (estante.Articulos != null && estante.Articulos.Any())
                            result = articulosPorEstanteRepository.Insert(estante.Articulos, idEstante, Convert.ToDateTime(estante.Creado), tran).Result;
                    }
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    idEstante = 0;
                }
            });
            t.Wait();
            return idEstante;
        }

        public override Task<bool> Update(Estante estante, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = OpenConnectionWithTransaction().Result;
            try
            {
                var t = Task.Run(() =>
                {
                    estante.Modificado = DateTime.Now;
                    var result = Update(estante, tran, columnsToIgnore).Result;
                    if (!result)
                        throw new Exception("Error al actualizar el estante");

                    if (estante.Decoraciones.Any())
                    {
                        result = decoracionRepository.DeleteFromEstante(estante.Id, tran).Result;
                        if (!result)
                            throw new Exception("Error al eliminar las decoraciones del estante");

                        result = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(estante.Id, tran).Result;
                        if (!result)
                            throw new Exception("Error al eliminar las decoraciones del estante");

                        result = decoracionRepository.Insert(estante.Decoraciones, estante.Id, Convert.ToDateTime(estante.Modificado), tran).Result;
                        if (!result)
                            throw new Exception("Error al insertar la decoración del estante");

                        if (estante.Articulos != null && estante.Articulos.Any())
                        {
                            result = articulosPorEstanteRepository.Insert(estante.Articulos, estante.Id, Convert.ToDateTime(estante.Modificado), tran).Result;
                            if (!result)
                                throw new Exception("Error al insertar llos artículos del estante");
                        }

                    }

                });
                t.Wait();
                tran.Commit();
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return Task.FromResult(false);
            }
        }

        public override Task<bool> Delete(Estante entity, string[] columnsToIgnore = null)
        {
            bool result = true;
            var t = Task.Run(() =>
            {
                using SqlTransaction tran = OpenConnectionWithTransaction().Result;
                try
                {

                    Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
                    Parameters = new Dictionary<string, object>() {
                        { "id", entity.Id },
                        { "modificado", DateTime.Now}
                    };

                    result = ExecuteInsertOrUpdate(Sql, Parameters, tran).Result;
                    if (result)
                        result = decoracionRepository.DeleteFromEstante(entity.Id, tran).Result;

                    if (result)
                        result = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(entity.Id, tran).Result;


                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result = false;
                }
            });

            return Task.FromResult(result);
        }

        public async Task<bool> DeleteFromGondola(long id)
        {
            var result = false;
            var t = Task.Run(() =>
            {
                using SqlTransaction tran = OpenConnectionWithTransaction().Result;
                try
                {
                    result = DeleteEstanteFromGondola(id, tran);


                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    id = 0;
                }
            });
            t.Wait();
            return result;
        }

        public async Task<bool> DeleteFromGondola(long id, SqlTransaction tran)
        {
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    result = DeleteEstanteFromGondola(id, tran);
                });
                t.Wait();
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Estante>> GetEstantesDeLaGondola(long id, SqlTransaction transaction = null)
        {
            Sql = "SELECT " + GetColumnsForSelect("es", new string[] { "Articulos", "Decoraciones" ,"Archivo"}) + " " + From + Where + "and id_gondola = @id_gondola";


            if (transaction == null)
                return await GetListOf<Estante>(Sql, new Dictionary<string, object>() { { "id_gondola", id } });
            else
                return await GetListOf<Estante>(Sql, transaction, new Dictionary<string, object>() { { "id_gondola", id } });
        }

        public override async Task<IEnumerable<Estante>> Get(string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";
            Where += " and id_gondola in (select id from gondola where eliminado = 0)";
            Sql = Select + From + Where;
            return await GetListOf<Estante>(Sql, new Dictionary<string, object>());
        }

        public override async Task<IEnumerable<Estante>> Get(string nombre, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";
            Where += " and id_gondola in (select id from gondola where eliminado = 0) and upper(" + Alias + ".nombre) like upper(@nombre)";

            Sql = Select + From + Where;
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<Estante>(Sql, Parameters);
        }

        private bool DeleteEstanteFromGondola(long id, SqlTransaction tran)
        {
            bool result;

            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_gondola = @id";
            Parameters = new Dictionary<string, object>() {
                { "id", id },
                { "modificado", DateTime.Now}
            };

            result = ExecuteInsertOrUpdate(Sql, Parameters, tran).Result;
            if (result)
                result = decoracionRepository.DeleteFromEstante(id, tran).Result;

            if (result)
                result = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(id, tran).Result;

            return result;
        }


    }
}
