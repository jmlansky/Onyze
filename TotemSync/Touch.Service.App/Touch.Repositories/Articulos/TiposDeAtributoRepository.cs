using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class TiposDeAtributoRepository : BaseRepository, ITiposDeAtributoRepository
    {
        public TiposDeAtributoRepository(IConfiguration configuration) : base(configuration)
        {
            Select = "select ta.id, ta.nombre, ta.creado, ta.modificado, ta.eliminado ";
            From = "from tipo_atributo ta ";
            Where = "where ta.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "update tipo_atributo " +
                "set eliminado = 1, " +
                "modificado = @modificado " +
                "Where id = @tipoAtributoID ";

            Parameters = new Dictionary<string, object>()
            {                
                { "@modificado", DateTime.Now },
                { "@tipoAtributoID", ((TipoAtributo)entity).Id }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<ComunEntity> Get(long id)
        {
            var sql = Select + From + Where + "and ta.id = @id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            return await Get<TipoAtributo>(sql, parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            var sql = Select + From + Where;
            return await GetListOf<TipoAtributo>(sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(ta.nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<TipoAtributo>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }
        
        public async Task<long> Insert(ComunEntity entity, SqlConnection connection)
        {
            Sql = "insert into tipo_atributo (nombre, creado, eliminado) values (@nombre, @creado, 0); Select SCOPE_IDENTITY() ";
            Parameters = new Dictionary<string, object>()
            {
                { "@nombre",  entity.Nombre},
                { "@creado", entity.Creado }
            };
            return Convert.ToInt64(await ExecuteScalarQuery(Sql, Parameters,false, connection));
        }       

        public async Task<bool> Insert(ComunEntity entity)
        {
            Sql = "insert into tipo_atributo (nombre, creado, eliminado) values (@nombre, @creado, 0) ";
            Parameters = new Dictionary<string, object>()
            {
                { "@nombre",  entity.Nombre},
                { "@creado", entity.Creado }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            Sql = "update tipo_atributo " +
                "set nombre = @nombre, " +
                "modificado = @modificado " +
                "Where id = @tipoAtributoID and eliminado = 0";

            Parameters = new Dictionary<string, object>()
            {
                { "@nombre",  entity.Nombre},
                { "@modificado", DateTime.Now },
                { "@tipoAtributoID", ((TipoAtributo)entity).Id }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
