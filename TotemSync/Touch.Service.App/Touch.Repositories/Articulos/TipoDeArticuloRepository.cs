using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class TipoDeArticuloRepository : BaseRepository, ITipoDeArticuloRepository
    {
        public TipoDeArticuloRepository(IConfiguration configuration) : base(configuration)
        {
            Select = "select t.id, t.nombre, t.creado, t.modificado, t.eliminado ";
            From = "from tipo_articulo t ";
            Where = "where t.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "update tipo_articulo set eliminado = 1, modificado = @modificado where id = @id";
            Parameters = new Dictionary<string, object>() 
            { 
                { "id", entity.Id },
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<ComunEntity> Get(long id)
        {
            Sql = Select + From + Where + "and t.id = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await Get<TipoArticulo>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<TipoArticulo>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(t.nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%"} };
            return await GetListOf<TipoArticulo>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            Sql = "insert into tipo_articulo (nombre, creado, eliminado) values (@nombre, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "creado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            Sql = "update tipo_articulo set nombre = @nombre, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id", entity.Id },
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
