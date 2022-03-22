using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Repositories.Comun
{
    public class BarriosRepository : BaseRepository, IBarriosRepository
    {
        public BarriosRepository(IConfiguration configuration): base(configuration)
        {
            Select = "SELECT ba.id, ba.nombre, ba.creado, ba.modificado, ba.eliminado, ba.id_localidad idLocalidad ";
            From = "FROM barrio ba ";
            Where = "WHERE ba.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "Update barrio set modificado = @modificado, eliminado = 1 where id = @id";
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
            return await Get<Barrio>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Barrio>(Sql, new Dictionary<string, object>());
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", "%" + nombre + "%" }
            };
            return await GetListOf<Barrio>(Sql, Parameters);
        }

        /// <summary>
        /// Get all localidades de una provincia
        /// </summary>
        /// <param name="id">id Localidad</param>
        /// <returns>Lista de "ComunEntity" que tienen que ser casteadas como barrio</returns>
        public async Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            Sql = Select + From + Where + "and id_localidad = @id_localidad";
            Parameters = new Dictionary<string, object>()
            {
                { "id_localidad", id}
            };
            return await GetListOf<Barrio>(Sql, Parameters);
        }

       

        public async Task<bool> Insert(ComunEntity entity)
        {
            var barrio = (Barrio)entity;
            Sql = "insert into barrio (nombre, id_localidad, creado, eliminado) values (@nombre, @id_localidad, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id_localidad", barrio.IdLocalidad},
                { "creado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            var barrio = (Barrio)entity;
            Sql = "Update localidad set nombre = @nombre, id_localidad = @id_localidad, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id},
                { "nombre", entity.Nombre},
                { "id_localidad", barrio.IdLocalidad},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
