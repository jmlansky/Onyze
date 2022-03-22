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
    public class FabricantesRepository: BaseRepository, IFabricantesRepository
    {
        public FabricantesRepository(IConfiguration configuration): base(configuration)
        {
            Select = "select f.id, f.nombre, f.creado, f.modificado, f.eliminado ";
            From = "from fabricante f ";
            Where = "where f.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "update fabricante set eliminado = 1, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>() 
            {
                { "id", entity.Id},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<ComunEntity> Get(long id)
        {
            Sql = Select + From + Where + "and f.id = @id";
            Parameters = new Dictionary<string, object>() 
            {
                { "id", id }
            };
            return await Get<Fabricante>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Fabricante>(Sql, new Dictionary<string, object>());
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(nombre) like upper(@nombre) ";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", "%" + nombre + "%"}
            };
            return await GetListOf<Fabricante>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<Fabricante> GetFabricante(long articuloId)
        {
            var sql = Select + From + ", articulo a " +
                Where + "and a.id = @articuloId and a.id_fabricante = f.id ";
            var parameters = new Dictionary<string, object>()
            {
                { "@articuloId", articuloId },
            };
            return await Get<Fabricante>(sql, parameters);
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            Sql = "insert into fabricante (nombre, creado, eliminado) values (@nombre, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {                
                { "nombre", entity.Nombre},
                { "creado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {            
            Sql = "update fabricante set nombre = @nombre, modificado = @modificado where id = @id and eliminado = 0";
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
