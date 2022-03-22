using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Repositories.Comun
{
    public class LocalidadesRepository : BaseRepository, ILocalidadesRepository
    {
        public LocalidadesRepository(IConfiguration configuration): base(configuration)
        {
            Select = "SELECT lo.id, lo.nombre, lo.creado, lo.modificado, lo.eliminado, lo.id_provincia idProvincia ";
            From = "FROM localidad lo ";
            Where = "WHERE lo.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "Update localidad set modificado = @modificado, eliminado = 1 where id = @id";
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
            return await Get<Localidad>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Localidad>(Sql, new Dictionary<string, object>());
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", "%" + nombre + "%" }
            };
            return await GetListOf<Localidad>(Sql, Parameters);
        }

        /// <summary>
        /// Get all localidades de una provincia
        /// </summary>
        /// <param name="id">id Provincia</param>
        /// <returns>Lista de "ComunEntity" que tienen que ser casteadas como localidades</returns>
        public async Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            Sql = Select + From + Where + "and id_provincia = @id_provincia";
            Parameters = new Dictionary<string, object>()
            {
                { "id_provincia", id}
            };
            return await GetListOf<Localidad>(Sql, Parameters);
        }

       

        public async Task<bool> Insert(ComunEntity entity)
        {
            var localidad = (Localidad)entity;
            Sql = "insert into localidad (nombre, id_provincia, creado, eliminado) values (@nombre, @id_provincia, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id_provincia", localidad.IdProvincia},
                { "creado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            var localidad = (Localidad)entity;
            Sql = "Update localidad set nombre = @nombre, id_provincia = @id_provincia, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id},
                { "nombre", entity.Nombre},
                { "id_provincia", localidad.IdProvincia},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
