using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class ArticulosRepository : BaseRepository, IArticulosRepository, ISincronizable<Articulo>
    {
        private readonly IGetArticulosRepository getArticulosRepository;
        public ArticulosRepository(IConfiguration configuration, IGetArticulosRepository getArticulosRepository) : base(configuration)
        {
            this.getArticulosRepository = getArticulosRepository;

            Select = "select ar.id, ar.nombre, ar.id_fabricante IdFabricante, ar.id_tipo idTipo, ar.descripcion, ar.descripcion_larga DescripcionLarga, " +
                "ar.etiquetas, ar.sku, ar.precio, ar.prospecto, ar.activo, ar.creado, ar.modificado, ar.eliminado ";

            From = "from articulo ar ";

            Where = "where ar.eliminado = 0 ";
        }

        #region Operaciones DELETE
        public async Task<bool> Delete(ComunEntity entity)
        {
            Sql = "update articulo set eliminado = 1, modificado = @modificado where id= @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id},
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public Task DeleteAlternativo(ArticuloMultiple articulo)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCruzado(ArticuloMultiple articulo)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Operaciones GET
        public async Task<ComunEntity> Get(long id)
        {
            Sql = Select + From + Where + "and id = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await Get<Articulo>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get()
        {
            Sql = Select + From + Where;
            return await GetListOf<Articulo>(Sql, Parameters);
        }

        public async Task<IEnumerable<ComunEntity>> Get(string nombre)
        {
            Sql = Select + From + Where +
                " and upper(ar.nombre) like upper('%" + @nombre + "%')";
            Parameters = new Dictionary<string, object>() { { "nombre", nombre } };
            return await GetListOf<Articulo>(Sql, Parameters);
        }

        public Task<IEnumerable<ComunEntity>> GetAll(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorAtributos(IEnumerable<string> nombres)
        {
            return await getArticulosRepository.GetArticulosPorAtributos(nombres);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorCategoria(string nombre)
        {
            return await getArticulosRepository.GetArticulosPorCategoria(nombre);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorCodigo(string ean)
        {
            return await getArticulosRepository.GetArticulosPorCodigo(ean);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorEstadoActivo(bool activo)
        {
            return await getArticulosRepository.GetArticulosPorEstadoActivo(activo);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorFabricante(string nombre)
        {
            return await getArticulosRepository.GetArticulosPorFabricante(nombre);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdCategoria(long id)
        {
            return await getArticulosRepository.GetArticulosPorIdCategoria(id);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdFabricante(long id)
        {
            return await getArticulosRepository.GetArticulosPorIdFabricante(id);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdTipo(long id)
        {
            return await getArticulosRepository.GetArticulosPorIdTipo(id);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorTipo(string nombre)
        {
            return await getArticulosRepository.GetArticulosPorTipo(nombre);
        }

        public async Task<IEnumerable<Articulo>> GetPorIds(List<long> idArticulos)
        {
            return await getArticulosRepository.GetPorIds(idArticulos);
        }

        public async Task<IEnumerable<Articulo>> GetPorSKU(string sku)
        {
            return await getArticulosRepository.GetPorSKU(sku);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorAtributos(string nombre)
        {
            return await getArticulosRepository.GetArticulosPorAtributos(nombre);
        }

        #endregion


        public async Task<bool> Insert(ComunEntity entity)
        {
            var articulo = (Articulo)entity;
            Sql = "insert into articulo (nombre, id_fabricante, id_tipo, descripcion, descripcion_larga, " +
                "etiquetas, sku, precio, prospecto, activo, creado, eliminado) " +
                "values (@nombre, @id_fabricante, @id_tipo, @descripcion, @descripcion_larga, " +
                "@etiquetas, @sku, @precio, @prospecto, @activo, @creado, 0)";
            Parameters = new Dictionary<string, object>()
            {
                {"nombre", articulo.Nombre },
                {"id_fabricante", articulo.IdFabricante },
                {"id_tipo", articulo.IdTipo },
                {"descripcion", articulo.Descripcion },
                {"descripcion_larga", articulo.DescripcionLarga },
                {"etiquetas", articulo.Etiquetas },
                {"@sku", articulo.SKU ?? string.Empty},
                {"precio", articulo.Precio },
                {"prospecto", articulo.Prospecto },
                {"activo", articulo.Activo },
                {"creado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters, false);
        }

        public async Task<long> InsertAndGetId(Articulo articulo)
        {
            Sql = "insert into articulo (nombre, id_fabricante, id_tipo, descripcion, descripcion_larga, " +
                "etiquetas, sku, precio, prospecto, activo, creado, eliminado) " +
                "values (@nombre, @id_fabricante, @id_tipo, @descripcion, @descripcion_larga, " +
                "@etiquetas, @sku, @precio, @prospecto, @activo, @creado, 0); Select SCOPE_IDENTITY()";
            Parameters = new Dictionary<string, object>()
            {
                {"nombre", articulo.Nombre },
                {"id_fabricante", articulo.IdFabricante },
                {"id_tipo", articulo.IdTipo },
                {"descripcion", articulo.Descripcion },
                {"descripcion_larga", articulo.DescripcionLarga },
                {"etiquetas", articulo.Etiquetas },
                {"@sku", articulo.SKU ?? string.Empty },
                {"precio", articulo.Precio },
                {"prospecto", articulo.Prospecto },
                {"activo", articulo.Activo },
                {"creado", DateTime.Now}
            };

            return Convert.ToInt64(await ExecuteScalarQuery(Sql, Parameters, false));
        }

        public async Task<bool> Update(ComunEntity entity)
        {
            var articulo = (Articulo)entity;
            Sql = "update articulo set nombre = @nombre, " +
                "id_fabricante = @id_fabricante, " +
                "id_tipo = @id_tipo, " +
                "descripcion = @descripcion, " +
                "descripcion_larga = @descripcion_larga, " +
                "etiquetas = @etiquetas, " +
                "sku = @sku, " +
                "precio = @precio, " +
                "prospecto = @prospecto, " +
                "activo = @activo, " +
                "modificado = @modificado, " +
                "eliminado = @eliminado " +
                "where id = @id " +
                "and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "id", articulo.Id },
                { "nombre", articulo.Nombre },
                { "id_fabricante", articulo.IdFabricante },
                { "id_tipo", articulo.IdTipo },
                { "descripcion", articulo.Descripcion },
                { "descripcion_larga", articulo.DescripcionLarga },
                { "etiquetas", articulo.Etiquetas },
                { "sku", articulo.SKU },
                { "precio", articulo.Precio },
                { "prospecto", articulo.Prospecto },
                { "activo", articulo.Activo },
                { "modificado", DateTime.Now},
                { "eliminado", articulo.Eliminado}
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<IEnumerable<Articulo>> GetWithDeleted(DateTime fechaSincro, string[] columnsToIgnore = null)
        {
            Sql = Select + From + " where creado > @fechasincro or (modificado is not null and modificado>@fechasincro) " +
                "ORDER BY CASE WHEN modificado is null  THEN creado ELSE modificado END";
            Parameters = new Dictionary<string, object>() { { "@fechasincro", fechaSincro } };
            return await GetListOf<Articulo>(Sql, Parameters);
        }

        public async Task<long> GetCurentCount()
        {
            Select = "SELECT count(*) ";
            Sql = Select + From + Where;

            return await GetValue<long>(Sql, Parameters);
        }
    }
}
