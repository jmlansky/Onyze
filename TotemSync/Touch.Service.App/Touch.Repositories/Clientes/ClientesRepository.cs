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
using Touch.Core.Clientes;
using Touch.Repositories.Clientes.Contracts;

namespace Touch.Repositories.Clientes
{
    public class ClientesRepository : SingleEntityComunRepository<Cliente>,  IClientesRepository
    {
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly ISingleEntityComunRepository<Barrio> barriosRepository;
        private readonly ISingleEntityComunRepository<Localidad> localidadRepository;
        private readonly ISingleEntityComunRepository<Provincia> provinciaRepository;

        public ClientesRepository(IConfiguration configuration,
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

        public override async Task<bool> Insert(Cliente entity, SqlTransaction sqlTransaction, string[] columnsToIgnore = null)
        {
            columnsToIgnore = new string[] { "Barrio", "Localidad", "Provincia", "Zona", "Sucursal" };
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

        public override async Task<long> InsertAndGetId(Cliente entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";

            long idCliente = 0;
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                idCliente = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));
                

                tran.Commit();

            }
            catch (Exception ex)
            {
                idCliente = 0;
                tran.Rollback();
            }

            return idCliente;
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


        public override async Task<IEnumerable<Cliente>> Get(string[] columnsToIgnore = null)
        {

           
            return await base.Get(columnsToIgnore?? new string[] { "Barrio", "Localidad", "Provincia", "Zona", "Sucursal", "Usuarios", "Sucursales", });
        }

        public override async Task<IEnumerable<Cliente>> Get(string nombre, string[] columnsToIgnore = null)
        {
            return await base.Get(nombre, columnsToIgnore ?? new string[] { "Barrio", "Localidad", "Provincia", "Zona", "Sucursal", "Usuarios" });
        }

        public override async Task<Cliente> Get(long id, string[] columnsToIgnore = null)
        {
            return await base.Get(id, columnsToIgnore ?? new string[] { "Barrio", "Localidad", "Provincia", "Zona", "Sucursal", "Usuarios" });
        }

        public override async Task<bool> Update(Cliente cliente, string[] columnsToIgnore = null)
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
                columnsToIgnore = new string[] { "Modificado", "Id", "Barrio", "Localidad" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Barrio").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Localidad").ToArray();
            }

            return columnsToIgnore;
        }

     
    }
}
