using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Publicaciones;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;
using Touch.Core.Comun;
using Touch.Core.Totems;
using Touch.Repositories.Totems.Contracts;
using Touch.Repositories.Sucursales.Contracts;


namespace Touch.Repositories.Totems
{
    public class TotemsRepository : SingleEntityComunRepository<Totem>, ITotemsRepository
    {
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly ISingleEntityComunRepository<Barrio> barriosRepository;
        private readonly ISingleEntityComunRepository<Localidad> localidadRepository;
        private readonly ISingleEntityComunRepository<Provincia> provinciaRepository;

        public TotemsRepository(IConfiguration configuration,
            ILocalidadesRepository localidadesRepository,
            ISingleEntityComunRepository<Barrio> barriosRepository,
            ISingleEntityComunRepository<Localidad> localidadRepository,
            ISingleEntityComunRepository<Provincia> provinciaRepository) : base(configuration)
        {
            this.localidadesRepository = localidadesRepository;
            this.barriosRepository = barriosRepository;
            this.localidadesRepository = localidadesRepository;
            this.provinciaRepository = provinciaRepository;
        }

        public override async Task<bool> Insert(Totem entity, SqlTransaction sqlTransaction, string[] columnsToIgnore = null)
        {
            columnsToIgnore = new string[] { "Sucursal" };
            entity.Creado = DateTime.Now;


            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                columnsToIgnore = GetColumnsToIgnore(columnsToIgnore);

                Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + "); " +
                    "Select SCOPE_IDENTITY()";
                Parameters = GetParameters(entity, columnsToIgnore);
                var result = Convert.ToInt64(await ExecuteScalarQuery(Sql, Parameters, false, tran.Connection, tran));



                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        public override async Task<long> InsertAndGetId(Totem entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";

            long idTotem = 0;
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                idTotem = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));

                entity.Id = idTotem;
                if (entity.IdSectores != null && entity.IdSectores.Any())
                    insertarSectoresDeTotem(entity, entity.Creado.Value, tran);

                tran.Commit();

            }
            catch (Exception ex)
            {
                idTotem = 0;
                tran.Rollback();
            }

            return idTotem;
        }

        private void insertarSectoresDeTotem(Totem entity, DateTime creado, SqlTransaction tran)
        {
            foreach (var sector in entity.IdSectores)
            {
                if (sector > 0)
                {
                    Sql = "Insert into totem_sector (id_totem, id_sector, creado, eliminado) values ( @id_totem,@id_sector,@creado,0) " +
                          "Select SCOPE_IDENTITY()";
                    Parameters = new Dictionary<string, object>() {
                    { "id_totem", entity.Id },
                    { "id_sector", sector  },
                    { "creado", creado}

                };

                    var result = Convert.ToInt64(ExecuteScalarQuery(Sql, Parameters, false, tran.Connection, tran).Result);

                    if (result == 0)
                        throw new Exception("Error al insertar sectores");
                }
            }



        }

        private static string[] GetColumnsToIgnoreForInsert(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            return columnsToIgnore;
        }


        public async Task<IEnumerable<Totem>> GetFromSucursal(long idCliente, long? idSucursal, string[] columnsToIgnore = null)
        {


            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + " INNER JOIN sucursal su on su.id = to0.id_sucursal " + Where + "and su.id_cliente = @id_cliente";
            Parameters = new Dictionary<string, object>() { { "id_cliente", idCliente } };

            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                Sql += " and to0.id_sucursal=@id_sucursal";
                Parameters.Add("id_sucursal", idSucursal);
            }



            return await GetListOf<Totem>(Sql, Parameters);

        }

        public async Task<IEnumerable<Totem>> Get(string nombre, long idCliente, long? idSucursal, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + " INNER JOIN sucursal su on su.id = to0.id_sucursal " + Where + " and su.eliminado=0 and su.id_cliente = @id_cliente and to0.nombre like  '%' + @nombre + '%' ";
            Parameters = new Dictionary<string, object>() { { "id_cliente", idCliente } };
            Parameters.Add("nombre", nombre);


            if (idSucursal.HasValue && idSucursal.Value > 0)
            {
                Sql += " and to0.id_sucursal=@id_sucursal";
                Parameters.Add("id_sucursal", idSucursal);
            }



            return await GetListOf<Totem>(Sql, Parameters);
        }

        public override async Task<Totem> Get(long id, string[] columnsToIgnore = null)
        {
            return await base.Get(id, columnsToIgnore ?? new string[] { "Usuarios" });
        }

        public override async Task<bool> Update(Totem totem, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {

                    EliminarSectoresTotem(totem, tran);
                    totem.Modificado = DateTime.Now;

                    result = Update(totem, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo actualizar el totem");


                    totem.Creado = totem.Modificado;

                    if (totem.IdSectores != null && totem.IdSectores.Any())
                        insertarSectoresDeTotem(totem, totem.Creado.Value, tran);


                });
                t.Wait();

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        private void EliminarSectoresTotem(Totem totem, SqlTransaction tran)
        {
            Sql = "delete from totem_sector where id_totem = @id_totem ";
            Parameters = new Dictionary<string, object>() {
                    { "id_totem", totem.Id },

                };

            var result = ExecuteNonQuery(Sql, Parameters, false, tran.Connection, tran).Result;

        }

        private static string[] GetColumnsToIgnore(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id", "Localidad" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Barrio").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Localidad").ToArray();
            }

            return columnsToIgnore;
        }

        public ICollection<Sector> GetSectoresFromTotem(long idTotem)
        {

            Select = "SELECT sec.id as Id, sec.nombre as Nombre, sec.creado as Creado, sec.modificado as modificado, sec.eliminado as Eliminado from totem_sector " +
                " inner join sector sec on totem_sector.id_sector = sec.id" +
                " WHERE  id_totem=@id_totem and sec.eliminado =0";

            Sql = Select;
            Parameters = new Dictionary<string, object>() { { "id_totem", idTotem } };


            return GetListOf<Sector>(Sql, Parameters).Result;
        }

        public override async Task<bool> Delete(Totem totem, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {

                    EliminarSectoresTotem(totem, tran);

                    Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
                    Parameters = new Dictionary<string, object>();
                    GetParameter(Parameters, totem, "id");
                    GetParameter(Parameters, totem, "modificado");
                    result = ExecuteInsertOrUpdate(Sql, Parameters, tran).Result;
                });
                t.Wait();

                tran.Commit();


                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }

        }
    }
}
