using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class GrillasRepository : SingleEntityComunRepository<Grilla>, IGrillasRepository
    {
        public GrillasRepository(IConfiguration configuration) : base(configuration)
        { }

        public Task DeleteFromGondola(long id, SqlTransaction tran)
        {
            var result = false;
            var t = Task.Run(() =>
            {
                Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_gondola = @id";
                Parameters = new Dictionary<string, object>() {
                    { "id", id },
                    { "modificado", DateTime.Now}
                };
                result = ExecuteInsertOrUpdate(Sql, Parameters).Result;
            });

            t.Wait();

            return Task.FromResult(result);
        }

        public Grilla GetFromGondola(long id)
        {
            Sql = Select + From + Where + " and id_gondola = @id";
            Parameters = new Dictionary<string, object>() {
                { "id", id }
            };
            return Get<Grilla>(Sql, Parameters).Result;
        }
    }
}
