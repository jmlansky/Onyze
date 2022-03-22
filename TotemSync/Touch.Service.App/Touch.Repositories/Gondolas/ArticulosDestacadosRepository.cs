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
    public class ArticulosDestacadosRepository: SingleEntityComunRepository<ArticuloDestacado>, IArticulosDestacadosRepository
    {
        public ArticulosDestacadosRepository(IConfiguration configuration): base(configuration)
        {}

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

        public async Task<ArticuloDestacado> GetFromArticulo(long idArticulo, long idEstante)
        {
            From = " FROM articulos_destacados ar, articulos_estantes ae ";
            Where = " WHERE ar.eliminado = 0 and ae.eliminado = 0 and ae.id_articulo = @id and id_estante = @id_estante and ae.id = ar.id_articulo_estante";

            Sql = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Destacado" }) + " " + From + Where;
            return await Get<ArticuloDestacado>(Sql, new Dictionary<string, object>() {
                { "id", idArticulo },
                { "id_estante", idEstante }
            });
        }
    }
}
