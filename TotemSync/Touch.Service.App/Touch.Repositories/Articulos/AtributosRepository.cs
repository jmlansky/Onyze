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
    public class AtributosRepository : BaseRepository, IAtributosRepository
    {
        private readonly ITiposDeAtributoRepository tiposDeAtributoRepository;

        public AtributosRepository(IConfiguration configuration, ITiposDeAtributoRepository tiposDeAtributoRepository) : base(configuration)
        {
            this.tiposDeAtributoRepository = tiposDeAtributoRepository;

            Select = "select at.id, at.nombre, at.id_tipo idTipo, at.creado, at.modificado, at.eliminado ";

            From = "from atributo at ";

            Where = "where at.eliminado = 0 ";
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "update atributo set eliminado = 1, modificado = @modificado where id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id },
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> DeleteAtributosDelArticulo(long id, long idAtributo)
        {
            Sql = "update atributos_articulos set modificado = @modificado, eliminado = 1 where id_articulo = @id_articulo and id_atributo = @id_atributo and eliminado = 0";
            Parameters = new Dictionary<string, object>
            {
                { "id_articulo", id},
                { "id_atributo", idAtributo},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> DeleteAtributosDelArticulo(long id)
        {
            Sql = "update atributos_articulos set modificado = @modificado, eliminado = 1 where id_articulo = @id_articulo and eliminado = 0";
            Parameters = new Dictionary<string, object>
            {
                { "id_articulo", id},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<ComunEntity> Get(long id)
        {
            var sql = Select + From + Where + " and at.id = @id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id}
            };
            return await Get<Atributo>(sql, parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Atributo>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where + "and upper(at.nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<Atributo>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Atributo>> GetAtributosDelArticulo(long articuloId)
        {
            var sql = Select + From + ", atributos_articulos apa "+
                Where + "and apa.id_articulo = @id and at.id = apa.id_atributo and apa.eliminado = 0";
            var parameters = new Dictionary<string, object>()
            {
                { "id", articuloId}
            };
            return await GetListOf<Atributo>(sql, parameters);
        }

        public async Task<IEnumerable<Atributo>> GetAtributosDelTipo(long idTipo)
        {
            var sql = Select + From + Where + "and at.id_tipo = @id_tipo and at.eliminado = 0";
            var parameters = new Dictionary<string, object>()
            {
                { "id_tipo", idTipo}
            };
            return await GetListOf<Atributo>(sql, parameters);
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            var atributo = (Atributo)entity;
            Sql = "insert into atributo (nombre, id_tipo, creado, eliminado) values (@nombre, @id_tipo, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id_tipo", atributo.IdTipo },
                { "creado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> InsertarAtributoDelArticulo(long id, long idAtributo)
        {
            Sql = "insert into atributos_articulos (id_articulo, id_atributo, creado, eliminado) values (@id_articulo, @id_atributo, @creado, 0)";
            Parameters = new Dictionary<string, object> 
            {
                { "id_articulo", id},
                { "id_atributo", idAtributo},
                { "creado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> InsertarAtributosDelArticulo(long articuloId, IEnumerable<Atributo> atributos)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                foreach (var atributo in atributos)
                {
                    if (atributo.TipoAtributo.Id > 0)
                        atributo.IdTipo = ((TipoAtributo)await tiposDeAtributoRepository.Get(atributo.TipoAtributo.Id)).Id;
                    else
                        atributo.IdTipo = await tiposDeAtributoRepository.Insert(atributo.TipoAtributo, tran.Connection);

                    if (atributo.IdTipo > 0)
                        await InsertarAtributo(tran, atributo, articuloId);

                    else throw new Exception();
                }
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }

            return true;
        }        

        public async Task<bool> Update(ComunEntity entity)
        {
            var atributo = (Atributo)entity;
            Sql = "update atributo set nombre = @nombre, id_tipo = @id_tipo, modificado = @modificado where id = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id_tipo", atributo.IdTipo },
                { "modificado", DateTime.Now },
                { "id", atributo.Id}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> UpdateAtributoDelArticulo(long id, long idAtributo)
        {
            Sql = "update atributos_articulos set id_atributo = @id_atributo, modificado = @modificado where id_articulo = @id_articulo and eliminado = 0";
            Parameters = new Dictionary<string, object>
            {
                { "id_articulo", id},
                { "id_atributo", idAtributo},
                { "modificado", DateTime.Now}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        private async Task InsertarAtributo(SqlTransaction tran, Atributo atributo, long articuloId)
        {
            Sql = "insert into atributo (nombre, creado, eliminado, id_tipo) values (@nombre, @creado, 0, @idTipo) returning id ";
            Parameters = new Dictionary<string, object>()
            {
                { "nombre", atributo.Nombre},
                { "creado", atributo.Creado},
                { "idTipo", atributo.IdTipo}
            };
            atributo.Id = Convert.ToInt64(await ExecuteScalarQuery(Sql, Parameters, false, tran.Connection));

            if (atributo.Id > 0)
                await InsertarAtributosPorArticulo(tran, atributo, articuloId);

            else throw new Exception();
        }

        private async Task InsertarAtributosPorArticulo(SqlTransaction tran, Atributo atributo, long articuloId)
        {
            Sql = "insert into atributos_articulos (id_articulo, id_atributo, creado, eliminado) " +
                "values (@idArticulo, @idAtributo, @creado, 0) ";
            Parameters = new Dictionary<string, object>()
            {
                { "idArticulo", articuloId},
                { "idAtributo", atributo.Id},
                { "creado", atributo.Creado}
            };
            if (await ExecuteInsertOrUpdate(Sql, Parameters, tran)) tran.Commit();
            else throw new Exception();
        }
    }
}
