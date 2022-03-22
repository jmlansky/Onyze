using Framework.Attributes;
using Framework.Helpers;
using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Repositories
{
    [QueryStatement("Select")]
    public class QuerySelect<T> : IQueryStatement<T> where T : new()
    {
        private readonly string[] columnsToIgnore;
        private string sql;
        private string select;
        private string from;
        private string where;
        private string alias;
        private readonly BaseRepository repository;
        //public QuerySelect(BaseRepository repository, string[] columnsToIgnore = null, string select = null, string from = null, string where = null, string alias = null, string sql = null)
        //{
        //    this.repository = repository;
        //    this.columnsToIgnore = columnsToIgnore;
        //    this.select = select;
        //    this.from = from;
        //    this.where = where;
        //    this.alias = alias;
        //    this.sql = sql;
        //}

        public async Task<IEnumerable<T>> Excecute(string sql)
        {
            if (columnsToIgnore != null && columnsToIgnore.Any())
                select = "SELECT " + ColumnProcessor<T>.GetColumnsForSelect(alias, columnsToIgnore) + " ";

            sql = select + from + where;
            var repository = (BaseRepository)InstancesHelper.GetInstanciaAplicar<ISingleEntityComunRepository<T>>("Totem");
            return await repository.GetListOf<T>(sql, new Dictionary<string, object>());
        }

        public Task<long> ExcecuteAndGetId(T entity, string[] columnsToIgnore = null)
        {
            throw new NotImplementedException();
        }
    }
}
