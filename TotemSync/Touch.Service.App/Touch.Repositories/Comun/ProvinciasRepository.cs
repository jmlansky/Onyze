using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Repositories.Comun
{
    public class ProvinciasRepository : BaseRepository, IProvinciasRepository
    {
        public ProvinciasRepository(IConfiguration configuration): base(configuration)
        {
            Select = "SELECT pr.id, pr.nombre, pr.creado, pr.modificado, pr.eliminado ";
            From = "FROM provincia pr ";
            Where = "WHERE pr.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "Update provincia set modificado = @modificado, eliminado = 1 where id = @id";
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
            return await Get<Provincia>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Provincia>(Sql, new Dictionary<string, object>());
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() 
            {
                { "nombre", "%" + nombre + "%" }
            };
            return await GetListOf<Provincia>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            Sql = "insert into provincia (nombre, creado, eliminado) values (@nombre, @creado, 0)";
            Parameters = new Dictionary<string, object>() 
            {
                { "nombre", entity.Nombre },
                { "creado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            Sql = "Update provincia set nombre = @nombre, modificado = @modificado where id = @id and eliminado = 0";
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
