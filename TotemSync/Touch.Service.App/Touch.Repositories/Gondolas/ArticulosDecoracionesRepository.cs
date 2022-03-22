using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;

namespace Touch.Repositories.Gondolas
{
    public class ArticulosDecoracionesRepository : SingleEntityComunRepository<ArticuloDecoracion>, IArticulosDecoracionesRepository
    {
        public ArticulosDecoracionesRepository(IConfiguration configuration) : base(configuration)
        { }

        public async Task<bool> DeleteFromIdArticuloEstante(long id, SqlTransaction tran)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_articulo_estante = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id},
                { "modificado", DateTime.Now}
            };

            return ExecuteInsertOrUpdate(Sql, Parameters, tran).Result;
        }

        public async Task<List<ArticuloDecoracion>> GetFromArticulo(long id, long idEstante)
        {
            From = " FROM articulos_decoraciones ar, articulos_estantes ae ";
            Where = " WHERE ar.eliminado = 0 and ae.eliminado = 0 and ar.id_articulo_estante = @id and ae.id_estante = @id_estante and ae.id = ar.id_articulo_estante";

            Sql = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Destacado","Archivo" }) + " " + From + Where;
            return GetListOf<ArticuloDecoracion>(Sql, new Dictionary<string, object>() {
                { "id", id },
                { "id_estante", idEstante }
            }).Result.ToList();
        }
    }
}
