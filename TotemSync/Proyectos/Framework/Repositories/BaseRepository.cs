using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Framework.Repositories
{
    public class BaseRepository
    {
        private readonly IConfiguration configuration;
        private string connectionString;

        public BaseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = GetConnectionString();
        }

        public string Sql { get; set; }
        public Dictionary<string, object> Parameters;
        public string Select { get; set; }
        public string From { get; set; }
        public string Where { get; set; }
        public string Alias { get; set; }
        public string Columns { get; set; }
        public string[] ColumnsToIgnore{ get; set; }

        private string GetConnectionString()
        {
            return configuration.GetSection("DB_CONNECTION_STRING").Value;
        }

        public async Task<SqlConnection> OpenConnection()
        {
            var sqlCon = new SqlConnection(connectionString);
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    await sqlCon.OpenAsync();
                    break;
                }
                catch (Exception ex)
                {
                    SqlConnection.ClearPool(sqlCon);
                }
            }
            if (sqlCon.State == ConnectionState.Open)
                return sqlCon;

            return null;
        }

        public async Task<SqlTransaction> OpenConnectionWithTransaction()
        {
            var sqlCon = new SqlConnection(connectionString);
            try
            {
                await sqlCon.OpenAsync();
                return sqlCon.BeginTransaction();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> ExecuteInsertOrUpdate(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            using (SqlTransaction tran = await OpenConnectionWithTransaction())
            {
                try
                {
                    if (await OnExecuteInsertOrUpdate(sql, parameters, isStoredProcedure, tran))
                    {
                        tran.Commit();
                        return true;
                    }
                    throw new Exception("Error al realizar la operación de insert o update");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    return false;
                }
            }
        }

        public async Task<bool> ExecuteInsertOrUpdate(string sql, Dictionary<string, object> parameters, SqlTransaction tran, bool isStoredProcedure = false)
        {
            return await OnExecuteInsertOrUpdate(sql, parameters, isStoredProcedure, tran);
        }

        private async Task<bool> OnExecuteInsertOrUpdate(string sql, Dictionary<string, object> parameters, bool isStoredProcedure, SqlTransaction tran)
        {
            try
            {
                if (string.IsNullOrEmpty(connectionString))
                    connectionString = GetConnectionString();

                await ExecuteNonQuery(sql, parameters, isStoredProcedure, tran.Connection, tran);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// This is used while trying to retreive 1 value like Id or bool or whatever. When the query uses the 'ExecuteScalar' command.
        /// </summary>
        /// <typeparam name="TResult">Expected type result</typeparam>
        /// <returns>return 1 value</returns>
        public async Task<TResult> GetValue<TResult>(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            using (SqlConnection con = await OpenConnection())
            {
                try
                {
                    object result = await ExecuteScalarQuery(sql, parameters, isStoredProcedure, con);
                    return (TResult)Convert.ChangeType(result, typeof(TResult));
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public async Task<object> ExecuteScalarQuery(string sql, Dictionary<string, object> parameters, bool isStoredProcedure, SqlConnection connection, SqlTransaction transaction = null)
        {
            var result = await CreateSqlCommand(sql, isStoredProcedure, connection, parameters, transaction).ExecuteScalarAsync();
            return result;
        }

        public async Task<object> ExecuteScalarQuery(string sql, Dictionary<string, object> parameters, bool isStoredProcedure)
        {
            var connection = await OpenConnection();
            return await CreateSqlCommand(sql, isStoredProcedure, connection, parameters).ExecuteScalarAsync();
        }

        public async Task<object> ExecuteNonQuery(string sql, Dictionary<string, object> parameters, bool isStoredProcedure, SqlConnection connection, SqlTransaction transaction = null)
        {
            var command = CreateSqlCommand(sql, isStoredProcedure, connection, parameters, transaction);
            return await command.ExecuteNonQueryAsync();
        }

        public async Task<ICollection<T>> GetListOf<T>(string sql, SqlTransaction transaction, Dictionary<string, object> parameters = null, bool isStoredProcedure = false) where T : new()
        {
            var list = new List<T>();

            try
            {
                SqlCommand cmd = CreateSqlCommand(sql, isStoredProcedure, transaction.Connection, parameters);
                cmd.Transaction = transaction;

                var dr = await cmd.ExecuteReaderAsync();
                while (dr.Read())
                {
                    var ob = Activator.CreateInstance<T>();
                    foreach (var property in ob.GetType().GetProperties())
                    {
                        if (dr.GetColumnSchema().Any(x => x.ColumnName.ToLower() == property.Name.ToLower()))
                        {
                            GetProperyWithValue(dr, ob, property);
                        }
                    }

                    list.Add(ob);
                }
                dr.Close();
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public async Task<ICollection<T>> GetListOf<T>(string sql, Dictionary<string, object> parameters = null, bool isStoredProcedure = false) where T : new()
        {
            var list = new List<T>();
            using (SqlConnection con = await OpenConnection())
            {
                try
                {
                    SqlCommand cmd = CreateSqlCommand(sql, isStoredProcedure, con, parameters);

                    var dr = await cmd.ExecuteReaderAsync();
                    while (dr.Read())
                    {
                        var ob = Activator.CreateInstance<T>();
                        foreach (var property in ob.GetType().GetProperties())
                        {
                            if (dr.GetColumnSchema().Any(x => x.ColumnName.ToLower() == property.Name.ToLower()))
                            {
                                GetProperyWithValue(dr, ob, property);
                            }
                        }

                        list.Add(ob);
                    }
                    dr.Close();
                    return list;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public async Task<T> Get<T>(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false) where T : new()
        {
            using (SqlConnection con = await OpenConnection())
            {
                try
                {
                    SqlCommand cmd = CreateSqlCommand(sql, isStoredProcedure, con, parameters);

                    var dr = await cmd.ExecuteReaderAsync();
                    var ob = Activator.CreateInstance<T>();
                    if (dr.Read())
                    {
                        foreach (var property in typeof(T).GetProperties())
                            GetProperyWithValue(dr, ob, property);
                    }
                    dr.Close();
                    return ob;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private void GetProperyWithValue<T>(SqlDataReader dr, T ob, PropertyInfo property) where T : new()
        {
            if (dr.GetColumnSchema().Any(x => x.ColumnName.ToLower() == property.Name.ToLower()))
            {
                var columnValue = dr[property.Name];
                if (columnValue.GetType().Name != "DBNull")
                    property.SetValue(ob, columnValue);
            }
        }

        private SqlCommand CreateSqlCommand(string sql, bool isStoredProcedure, SqlConnection connection, Dictionary<string, object> parameters = null, SqlTransaction transaction = null)
        {
            var sqlCommand = new SqlCommand()
            {
                Connection = connection,
                CommandText = sql,
                CommandType = isStoredProcedure ? CommandType.StoredProcedure : CommandType.Text,
                Transaction = transaction
            };

            if (parameters != null && parameters.Any())
            {
                foreach (var parameter in parameters)
                    sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }

            return sqlCommand;
        }
    }
}
