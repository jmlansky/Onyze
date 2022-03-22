using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Repositories.Comun
{
    public class ZonasRepository : BaseRepository, IZonasRepository
    {
        public ZonasRepository(IConfiguration configuration): base (configuration)
        {
            Select = "SELECT zo.id, zo.nombre, zo.creado, zo.modificado, zo.eliminado ";
            From = "FROM zona zo ";
            Where = "WHERE zo.eliminado = 0 ";
        }        

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "Update zona set modificado = @modificado, eliminado = 1 where id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<ComunEntity> Get(long id)
        {
            Sql = Select + From + Where + "and id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            return await Get<Zona>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Zona>(Sql, new Dictionary<string, object>());
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(zo.nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", "%" + nombre + "%" }
            };
            return await GetListOf<Zona>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            Sql = "insert into zona (nombre, creado, eliminado) values (@nombre, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "creado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            Sql = "Update zona set nombre = @nombre, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id},
                { "nombre", entity.Nombre},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
