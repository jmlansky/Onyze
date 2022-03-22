using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Touch.Core.Attributes;
using Touch.Core.Promociones;

namespace Touch.Repositories.Comun
{
    public class SingleEntityComunRepository<T> : BaseRepository, ISingleEntityComunRepository<T> where T : new()
    {

        public SingleEntityComunRepository(IConfiguration configuration) : base(configuration)
        {
            Alias = typeof(T).Name.Substring(0, 2).ToLower() ;

            string[] reserved = new string[] { "AS", "BY", "IF", "IN", "IS", "TO", "OF", "ON", "OR" };

            if (reserved.Contains(Alias.ToUpper()))
                Alias += "0";

            Columns = GetColumnsForSelect(Alias);
            Select = "SELECT " + Columns + " ";
            From = "FROM " + GetTableName() + " " + Alias + " ";
            Where = "WHERE " + Alias + ".eliminado = 0 ";
        }

        public virtual async Task<IEnumerable<T>> Get(string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where;
            return await GetListOf<T>(Sql, new Dictionary<string, object>());
        }

        public virtual async Task<T> Get(long id, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and id = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await Get<T>(Sql, Parameters);
        }
        public virtual async Task<IEnumerable<T>> Get(string nombre, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            Sql = Select + From + Where + "and upper(" + Alias + ".nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<T>(Sql, Parameters);
        }

        public virtual async Task<IEnumerable<T>> Get(List<long> ids, string[] columnsToIgnore = null)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";            
            var parameters = new Dictionary<string, object>();
            Sql = Select + From + Where + " and (ar.id = @id0";
            parameters.Add("id0", ids.FirstOrDefault());

            for (int i = 1; i < ids.Count; i++)
            {
                Sql += " or ar.id = @id" + i.ToString();
                parameters.Add("id" + i.ToString(), ids[i]);
            }
            Sql += ")";
            return await GetListOf<T>(Sql, parameters);
        }

        public virtual async Task<bool> Insert(T entity, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ")";
            return await ExecuteInsertOrUpdate(Sql, GetParameters(entity, columnsToIgnore));
        }

        public virtual async Task<bool> Insert(T entity, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ")";
            return await ExecuteInsertOrUpdate(Sql, GetParameters(entity, columnsToIgnore), tran);
        }

        public virtual async Task<bool> Update(T entity, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Creado" };
            else
                columnsToIgnore = columnsToIgnore.Append("Creado").ToArray();

            Sql = "UPDATE " + GetTableName() + " SET " + GetColumnsForUpdate(columnsToIgnore) + " WHERE eliminado = 0 and id = @id";
            return await ExecuteInsertOrUpdate(Sql, GetParameters(entity, columnsToIgnore));
        }

        public async Task<bool> Update(T entity, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Creado" };
            else
                columnsToIgnore = columnsToIgnore.Append("Creado").ToArray();

            Sql = "UPDATE " + GetTableName() + " SET " + GetColumnsForUpdate(columnsToIgnore) + " WHERE eliminado = 0 and id = @id";
            return await ExecuteInsertOrUpdate(Sql, GetParameters(entity, columnsToIgnore), tran);
        }

        public virtual async Task<bool> Delete(T entity, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
            Parameters = new Dictionary<string, object>();
            GetParameter(Parameters, entity, "id");
            GetParameter(Parameters, entity, "modificado");
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public virtual async Task<bool> Delete(T entity, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
            Parameters = new Dictionary<string, object>();
            GetParameter(Parameters, entity, "id");
            GetParameter(Parameters, entity, "modificado");

            return await ExecuteInsertOrUpdate(Sql, Parameters, tran);
        }

        public string GetTableName()
        {
            var tableAttribute = (typeof(T).GetCustomAttributes(true).FirstOrDefault() as TableNameAttribute);
            if (tableAttribute != null)
                return tableAttribute.TableName;
            return typeof(T).Name.ToLower();
        }

        protected string GetColumnsForSelect(string alias, string[] columnsToIgnore = null)
        {
            var columns = new List<string>();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null)
                {
                    if (columnsToIgnore.Contains(prop.Name))
                        continue;
                }

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null)
                    columns.Add(alias + "." + column.ColumnName + " as " + prop.Name);
                else
                    columns.Add(alias + "." + prop.Name);
            }
            var result = string.Join(", ", columns);
            return result;
        }

        public string GetColumnsForInsert(string[] columnsToIgnore = null)
        {
            var columns = new List<string>();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null)
                {
                    if (columnsToIgnore.Contains(prop.Name))
                        continue;
                }

                if (prop.Name.ToLower().Equals("id") || prop.Name.ToLower().Equals("modificado"))
                    continue;

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null)
                    columns.Add(column.ColumnName);
                else
                    columns.Add(prop.Name);
            }
            var result = string.Join(", ", columns);
            return result;
        }

        public string GetColumnsForUpdate(string[] columnsToIgnore = null)
        {
            var columns = new List<string>();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null)
                {
                    if (columnsToIgnore.Contains(prop.Name))
                        continue;
                }

                if (prop.Name.ToLower().Equals("id") || prop.Name.ToLower().Equals("creado"))
                    continue;

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;

                if (column != null)
                    columns.Add(column.ColumnName + " = @" + column.ColumnName);
                else
                    columns.Add(prop.Name + " = @" + prop.Name);
            }
            var result = string.Join(", ", columns);
            return result;
        }

        public string GetParametersString(string[] columnsToIgnore = null)
        {
            var parameters = new List<string>();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null)
                {
                    if (columnsToIgnore.Contains(prop.Name))
                        continue;
                }

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null)
                    parameters.Add("@" + column.ColumnName);
                else
                    parameters.Add("@" + prop.Name);
            }
            var result = string.Join(", ", parameters);
            return result;
        }

        public Dictionary<string, object> GetParameters(T entity, string[] columnsToIgnore = null)
        {
            var parameters = new Dictionary<string, object>();

            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                if (columnsToIgnore != null)
                {
                    if (columnsToIgnore.Contains(prop.Name))
                        continue;
                }

                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null) {
                    var value = prop.GetValue(entity);
                    if (value == null)
                        value = DBNull.Value;
                    parameters.Add(column.ColumnName, value);
                }
                else
                    parameters.Add(prop.Name, prop.GetValue(entity));
            }

            return parameters;
        }

        public void GetParameter(Dictionary<string, object> parameters, T entity, string columnName)
        {
            PropertyInfo prop = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals(columnName.ToLower()));
            if (prop != null)
            {
                var column = prop.GetCustomAttributes(true).FirstOrDefault() as ColumnNameAttribute;
                if (column != null)
                    parameters.Add(column.ColumnName, prop.GetValue(entity));
                else
                    parameters.Add(prop.Name, prop.GetValue(entity));
            }
        }

        public virtual async Task<long> InsertAndGetId(T entity, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";
            return Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false));
        }

        public virtual async Task<long> InsertAndGetId(T entity, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";
            return Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran ));
        }        
    }
}
