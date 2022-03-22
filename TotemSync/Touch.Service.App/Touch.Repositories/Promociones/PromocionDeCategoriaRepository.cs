using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Promociones
{
    public class PromocionDeCategoriaRepository : SingleEntityComunRepository<PromocionDeCategoria>, IPromocionDeCategoriaRepository
    {
        public PromocionDeCategoriaRepository(IConfiguration configuration) : base(configuration)
        { }

        public async Task<bool> DeleteFromPromocion(long idPromocion, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", idPromocion},
                { "modificado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters, tran);
        }

        public async Task<IEnumerable<PromocionDeCategoria>> GetPorPromocion(long idPromocion, string[] columnsToIgnore = null)
        {
            Sql = "Select " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + " and id_promocion = @id_promocion";
            return await GetListOf<PromocionDeCategoria>(Sql, new Dictionary<string, object> { { "id_promocion", idPromocion } });
        }
    }
}
