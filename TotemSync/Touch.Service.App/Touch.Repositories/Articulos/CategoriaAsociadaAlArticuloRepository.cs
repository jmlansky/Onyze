using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class CategoriaAsociadaAlArticuloRepository : SingleEntityComunRepository<CategoriaAsociadaAlArticulo>, ICategoriaAsociadaAlArticuloRepository
    {
        public CategoriaAsociadaAlArticuloRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public bool DeleteCategoriasDelArticulo(long id)
        {
            var sql = "update categorias_articulos set eliminado = 1, modificado = @modificado where id_articulo = @id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id},
                { "modificado", DateTime.Now}
            };
            return ExecuteInsertOrUpdate(sql, parameters).Result;
        }

        public async Task DeleteCategoriasDelArticulo(long id, IEnumerable<long> categorias)
        {
            foreach (var idCategoria in categorias)
            {
                var sql = "update categorias_articulos set eliminado = 1, modificado = @modificado where id_articulo = @id and id_categoria = @id_categoria";
                var parameters = new Dictionary<string, object>()
                {
                    { "id", id},
                    { "modificado", DateTime.Now},
                    { "id_categoria", idCategoria}
                };
                await ExecuteInsertOrUpdate(sql, parameters);
            }
        }

        public IEnumerable<CategoriaDeArticulo> GetCategoriasDelArticulo(long articuloId)
        {
            var sql = "select c.id, c.nombre " + From + ", categoria c " +
                   Where + " and c.id = ca.id_categoria and c.eliminado = 0 and ca.id_articulo = @id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", articuloId}
            };
            return GetListOf<CategoriaDeArticulo>(sql, parameters).Result;
        }
    }
}


