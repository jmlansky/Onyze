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
using Touch.Repositories.Sucursales.Contracts;
using Touch.Core.Sucursales;

namespace Touch.Repositories.Sucursales
{
    public class SucursalesRepository : SingleEntityComunRepository<Sucursal>, ISucursalesRepository
    {
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly ISingleEntityComunRepository<Barrio> barriosRepository;
        private readonly ISingleEntityComunRepository<Localidad> localidadRepository;
        private readonly ISingleEntityComunRepository<Provincia> provinciaRepository;

        public SucursalesRepository(IConfiguration configuration,
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

        public override async Task<bool> Insert(Sucursal entity, SqlTransaction sqlTransaction, string[] columnsToIgnore = null)
        {
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

        public override async Task<long> InsertAndGetId(Sucursal entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";

            long idSucursal = 0;
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                idSucursal = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));


                tran.Commit();

            }
            catch (Exception ex)
            {
                idSucursal = 0;
                tran.Rollback();
            }

            return idSucursal;
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


        public async Task<IEnumerable<Sucursal>> Get(long idCliente, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and id_cliente = @id_cliente";
            Parameters = new Dictionary<string, object>() { { "id_cliente", idCliente } };
            return await GetListOf<Sucursal>(Sql, Parameters);

        }

        public async Task<IEnumerable<Sucursal>> Get(string nombre, long idCliente, string[] columnsToIgnore = null)
        {

            Select = "SELECT " + GetColumnsForSelect(Alias, GetColumnsToIgnore(columnsToIgnore)) + " ";

            Sql = Select + From + Where + "and id_cliente = @id_cliente and (nombre like  '%' + @nombre + '%' or calle like '%' + @nombre + '%'  OR nombre_referente like '%' + @nombre + '%')";
            Parameters = new Dictionary<string, object>() { { "id_cliente", idCliente }, { "nombre", nombre ?? "" } };
            return await GetListOf<Sucursal>(Sql, Parameters);


        }

        public async Task<Sucursal> Get(long id, long idCliente, string[] columnsToIgnore = null)
        {

            Select = "SELECT " + GetColumnsForSelect(Alias, GetColumnsToIgnore(columnsToIgnore)) + " ";

            Sql = Select + From + Where + "and su.id_cliente = @id_cliente and su.id=@id";
            Parameters = new Dictionary<string, object>() { { "id_cliente", idCliente }, { "id", id } };

            return await Get<Sucursal>(Sql, Parameters);
        }

        public override async Task<bool> Update(Sucursal cliente, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {

                    cliente.Modificado = DateTime.Now;
                    result = Update(cliente, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo actualizar el cliente");

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






        private static string[] GetColumnsToIgnore(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Cliente", "Barrio", "Localidad", "Provincia", "Zona", "Sucursal", "Usuarios", "Region" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Barrio").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Localidad").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Cliente").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Provincia").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Zona").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Sucursal").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Usuarios").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Region").ToArray();
            }

            return columnsToIgnore;
        }


    }
}
