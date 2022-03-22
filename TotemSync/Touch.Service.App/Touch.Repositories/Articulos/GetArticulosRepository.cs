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
    public class GetArticulosRepository : BaseRepository, IGetArticulosRepository
    {
        public GetArticulosRepository(IConfiguration configuration) : base(configuration)
        {
            Select = "select ar.id, ar.nombre, ar.id_fabricante IdFabricante, ar.id_tipo idTipo, ar.descripcion, ar.descripcion_larga DescripcionLarga, " +
                "ar.etiquetas, ar.sku, ar.precio, ar.prospecto, ar.activo, ar.creado, ar.modificado, ar.eliminado ";

            From = "from articulo ar ";
            Where = "Where ar.eliminado = 0 ";
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorAtributos(IEnumerable<string> nombres)
        {
            Sql = Select + From + ", atributo at, atributos_articulos aa " +
                    Where + "and aa.id_articulo = ar.id and aa.id_atributo = at.id and aa.eliminado = 0 ";

            Parameters = new Dictionary<string, object>();
            var arrayDeNombres = nombres.ToArray();
            for (int i = 0; i < nombres.Count(); i++)
            {
                var paramName = "@nombre" + i.ToString();
                Sql += "and upper(at.nombre) LIKE upper(" + paramName + ") ";
                Parameters.Add(paramName, "%" + arrayDeNombres[i] + "%");
            }

            return await GetListOf<Articulo>(Sql, Parameters); ;
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorAtributos(string nombre)
        {
            //nombre, fabricante, etiquetas, sku, ean (codigo de barra)
            Select = @"select distinct ar.id, ar.nombre, ar.id_fabricante IdFabricante, ar.id_tipo idTipo, ar.descripcion, ar.descripcion_larga DescripcionLarga, " +
                "ar.etiquetas, ar.sku, ar.precio, ar.prospecto, ar.activo, ar.creado, ar.modificado, ar.eliminado ";

            From += @" left outer join fabricante fa on  ar.id_fabricante = fa.id left outer join codigo_barra cb on cb.id_articulo = ar.id ";
            Where = @"Where ar.eliminado = 0 
                and (ar.nombre like @nombre or ar.sku like @nombre or fa.nombre like @nombre or ar.etiquetas like @nombre or cb.ean like @nombre) ";
            Sql = Select + From + Where;

            Parameters = new Dictionary<string, object>()  { { "nombre", "%" + nombre + "%"} };            
            return await GetListOf<Articulo>(Sql, Parameters); ;
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorCategoria(string nombre)
        {
            var sql = Select + From + ", categoria c, categorias_articulos cxa " +
                    Where + "and ar.id = cxa.id_articulo " +
                    "and c.id = cxa.id_categoria " +
                    "and upper(c.nombre) like upper('%" + @nombre + "%')";

            var parameters = new Dictionary<string, object>()
            {
                { "nombre", nombre },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorCodigo(string ean)
        {
            var sql = Select + From + ", codigo_barra cdb " +
                    Where + "and ar.id = cdb.id_articulo and cdb.eliminado = 0 " +
                    "and upper(cdb.ean) like upper('%" + @ean + "%')";

            Parameters = new Dictionary<string, object>()
            {
                { "ean", ean },
            };
            return await GetListOf<Articulo>(sql, Parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorEstadoActivo(bool activo)
        {
            var sql = Select + From + ", codigo_barra cdb " +
                    Where + "and ar.id = cdb.id_articulo and cdb.eliminado = 0 " +
                    "and activo = @activo";

            Parameters = new Dictionary<string, object>()
            {
                { "activo", Convert.ToInt16(activo)},
            };
            return await GetListOf<Articulo>(sql, Parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorFabricante(string nombre)
        {
            var sql = Select +
                From + ", fabricante f " +
                Where + "and upper(f.nombre) like upper('%" + @nombre + "%') and ar.id_fabricante = f.id";

            var parameters = new Dictionary<string, object>()
            {
                { "nombre", nombre },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdCategoria(long id)
        {
            var sql = Select +
                    From + ", categoria c, categorias_articulos cxa " +
                    Where + "and ar.id = cxa.id_articulo " +
                       "and c.id = cxa.id_categoria " +
                       "and c.id = @id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdFabricante(long id)
        {
            var sql = Select +
                From + ", fabricante f " +
                Where + "and ar.id_fabricante = @id and ar.id_fabricante = f.id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorIdTipo(long id)
        {
            var sql = Select +
                From + ", tipo_articulo t " +
                Where + "and ar.id_tipo = @id and ar.id_tipo = t.id";
            var parameters = new Dictionary<string, object>()
            {
                { "id", id },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetArticulosPorTipo(string nombre)
        {
            var sql = "Select ar.* " +
                From + ", tipo_articulo t " +
                Where + "and upper(t.nombre) like upper('%" + @nombre + "%') and ar.id_tipo = t.id";
            var parameters = new Dictionary<string, object>()
            {
                { "nombre", nombre },
            };
            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetPorIds(List<long> idArticulos)
        {
            var sql = @"select ar.* 
                        from articulo ar 
                        where eliminado = 0";

            var parameters = new Dictionary<string, object>();
            sql += " and ar.id = @id0";
            parameters.Add("id0", idArticulos.FirstOrDefault());

            for (int i = 1; i < idArticulos.Count; i++)
            {
                sql += " or ar.id = @id" + i.ToString();
                parameters.Add("id" + i.ToString(), idArticulos[i]);
            }

            return await GetListOf<Articulo>(sql, parameters);
        }

        public async Task<IEnumerable<Articulo>> GetPorSKU(string sku)
        {
            Sql = Select + From + Where + "and upper(ar.sku) like upper('%" + @sku + "%')";
            Parameters = new Dictionary<string, object>() { { "sku", sku } };
            return await GetListOf<Articulo>(Sql, Parameters);
        }
    }
}
