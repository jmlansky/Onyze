using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class CodigosDeBarraRepository : SingleEntityComunRepository<CodigoDeBarras>, ICodigosDeBarraRepository
    {
        public CodigosDeBarraRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> DeleteCodigosDelArticulo(long id)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado where eliminado = 0 and id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>() 
            {
                { "modificado", DateTime.Now},
                { "id_articulo", id}
            };
            
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<IEnumerable<CodigoDeBarras>> Get(string ean, string[] columnsToIgnore = null)
        {
            Sql = Select + From + Where + "and upper(co.ean) like upper(@ean)";
            Parameters = new Dictionary<string, object>() { { "ean", "%" + ean +"%"} };
            return await GetListOf<CodigoDeBarras>(Sql, Parameters);
        }

        public async Task<IEnumerable<CodigoDeBarras>> GetCodigosDelArticulo(long id)
        {
            Sql = Select + From + Where + "and id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>() { { "id_articulo", id } };
            return await GetListOf<CodigoDeBarras>(Sql, Parameters);
        }
    }
}
