using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class ArticuloMultipleRepository : BaseRepository, IArticuloMultipleRepository
    {
        public string NombreTabla { get; set; }
        public ArticuloMultipleRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> Delete(ComunEntity entity)
        {
            try
            {
                var articulo = (ArticuloMultiple)entity;
                Sql = "update " + NombreTabla + " set eliminado = 1, modificado = @modificado where id_origen = @id_origen and id_destino = @id_destino and eliminado = 0";
                Parameters = new Dictionary<string, object>() {
                    { "id_origen", articulo.IdOrigen },
                    { "id_destino", articulo.IdDestino },
                    { "modificado", DateTime.Now},
                };
                return await ExecuteInsertOrUpdate(Sql, Parameters);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ComunEntity> Get(long id)
        {
            Sql = "SELECT id, id_origen idOrigen, id_destino idDestino FROM " + NombreTabla + " where id = @id and eliminado = 0 ";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await Get<ArticuloMultiple>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            Sql = "SELECT id, id_origen idOrigen, id_destino idDestino FROM " + NombreTabla + " where id_origen = @id and eliminado = 0";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await GetListOf<ArticuloMultiple>(Sql, Parameters);
        }

        public async Task<bool> Insert(ComunEntity entity)
        {
            try
            {
                var articulo = (ArticuloMultiple)entity;
                Sql = "insert into " + NombreTabla + " (id_origen, id_destino, creado) values (@from, @to, @creado)";
                Parameters = new Dictionary<string, object>()
                {
                    {"from", articulo.IdOrigen },
                    {"to", articulo.IdDestino },
                    {"creado", DateTime.Now },
                };
                return await ExecuteInsertOrUpdate(Sql, Parameters);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            try
            {
                var articulo = (ArticuloMultiple)entity;
                Sql = "update " + NombreTabla + " set id_origen = @from, id_destino = @to, modificado = @modificado where id = @id";
                Parameters = new Dictionary<string, object>()
                {
                    { "from", articulo.IdOrigen },
                    { "to", articulo.IdDestino },
                    { "modificado", DateTime.Now},
                    { "id", articulo.Id}
                };
                return await ExecuteInsertOrUpdate(Sql, Parameters);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetNombreTabla(string nombreTabla)
        {
            NombreTabla = nombreTabla;
        }

        public Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeletAllFromArticulo(long id)
        {
            try
            {
                Sql = "update " + NombreTabla + " set eliminado = 1, modificado = @modificado where id_origen = @id and eliminado = 0";
                Parameters = new Dictionary<string, object>() {
                    { "id", id },
                    { "modificado", DateTime.Now},
                };
                return await ExecuteInsertOrUpdate(Sql, Parameters);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Relacion>> GetRelaciones(DateTime fechaSincro, string[] columnsToIgnore = null)
        {
            Sql = @"select * from (
select id as Id, id_origen as IdOrigen, id_destino as IdDestino, creado as Creado, modificado as Modificado, 'alternativo' as Tipo, eliminado as Eliminado  from articulos_alternativos  where creado > @fechasincro or (modificado is not null and modificado > @fechasincro)
Union
select id as Id, id_origen as IdOrigen, id_destino as IdDestino, creado as Creado, modificado as Modificado, 'cruzado' as Tipo , eliminado as Eliminado from articulos_cruzados  where creado > @fechasincro or (modificado is not null and modificado > @fechasincro))
x
ORDER BY CASE WHEN Modificado is null  THEN Creado ELSE Modificado END
";
            Parameters = new Dictionary<string, object>() { { "@fechasincro", fechaSincro } };
            return await GetListOf<Relacion>(Sql, Parameters);
        }
    }
}
