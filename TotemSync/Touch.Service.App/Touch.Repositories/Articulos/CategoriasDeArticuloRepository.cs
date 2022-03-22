using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class CategoriasDeArticuloRepository : SingleEntityComunRepository<CategoriaDeArticulo>, ICategoriasDeArticuloRepository
    {
        public CategoriasDeArticuloRepository(IConfiguration configuration) : base(configuration)
        {
            Select = "Select cat.id, cat.nombre, cat.creado, cat.modificado, cat.eliminado, cat.id_categoria_padre idCategoriaPadre, cat.id_archivo idArchivo, cat.mostrar_totem mostrarEnTotem ";
            From = "From categoria cat ";
            Where = "Where cat.eliminado = 0 ";
        }

        public async Task<bool> AsociarArchivoCategoria(CategoriaDeArticulo categoria)
        {
            Sql = "update categoria set id_archivo = @id_archivo where id = @id and eliminado = 0";

            Parameters = new Dictionary<string, object>()
            {
                { "id_archivo", categoria.IdArchivo },
                { "id", categoria.Id }
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<bool> Delete(CategoriaDeArticulo entity, string[] columnsToIgnore = null)
        {
            Sql = "update categoria set eliminado = 1, modificado = @modificado where id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", entity.Id },
                { "modificado", DateTime.Now }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public CategoriaDeArticulo EsArchivoDeAlgunaCategoria(long id)
        {
            Sql = Select + From + Where + "and cat.id_archivo = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return Get<CategoriaDeArticulo>(Sql, Parameters).Result;
        }

        public override async Task<CategoriaDeArticulo> Get(long id, string[] columnsToIgnore = null)
        {
            Sql = Select + From + Where + "and cat.id = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return await Get<CategoriaDeArticulo>(Sql, Parameters);
        }

        public override async Task<IEnumerable<CategoriaDeArticulo>> Get(string[] columnsToIgnore = null)
        {
            Sql = Select + From + Where;
            return await GetListOf<CategoriaDeArticulo>(Sql, Parameters);
        }

        public override async Task<IEnumerable<CategoriaDeArticulo>> Get(string nombre, string[] columnsToIgnore = null)
        {
            Sql = Select + From + Where + "and upper(cat.nombre) like upper(@nombre)";
            Parameters = new Dictionary<string, object>() { { "nombre", "%" + nombre + "%" } };
            return await GetListOf<CategoriaDeArticulo>(Sql, Parameters);
        }


        public async Task<IEnumerable<CategoriaDeArticulo>> GetCategoriasDelArticulo(long articuloId)
        {
            Sql = Select
                + From + ", categorias_articulos cxa "
                + Where + "and cxa.id_articulo = @id and cxa.id_categoria = cat.id ";
            Parameters = new Dictionary<string, object>() { { "id", articuloId } };
            return await GetListOf<CategoriaDeArticulo>(Sql, Parameters);
        }

        public override async Task<bool> Insert(CategoriaDeArticulo entity, string[] columnsToIgnore = null)
        {
            var categoria = (CategoriaDeArticulo)entity;
            var parametersString = "@nombre, @creado, 0";
            var insertcolumnsString = "nombre, creado, eliminado";

            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "creado", DateTime.Now }
            };

            if (categoria.IdCategoriaPadre.HasValue && categoria.IdCategoriaPadre > 0)
            {
                insertcolumnsString += ", id_categoria_padre";
                parametersString += ", @id_categoria_padre";
                Parameters.Add("id_categoria_padre", categoria.IdCategoriaPadre);
            }

            if (categoria.IdArchivo.HasValue && categoria.IdArchivo > 0)
            {
                insertcolumnsString += ", id_archivo";
                parametersString += ", @id_archivo";
                Parameters.Add("id_archivo", categoria.IdArchivo);
            }

            Sql = "insert into categoria (" + insertcolumnsString + ") values (" + parametersString + ")";
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<bool> Update(CategoriaDeArticulo entity, string[] columnsToIgnore = null)
        {
            Sql = "update categoria set nombre = @nombre, modificado = @modificado, mostrar_totem=@mostrar_totem ";
            var where = "where id = @id and eliminado = 0";

            Parameters = new Dictionary<string, object>()
            {
                { "nombre", entity.Nombre },
                { "id", entity.Id },
                { "modificado", DateTime.Now },
                { "mostrar_totem", entity.MostrarEnTotem }
            };

            var categoria = (CategoriaDeArticulo)entity;

            if (categoria.IdCategoriaPadre == 0)
            {

                Sql += ", id_categoria_padre = @id_categoria_padre ";
                Parameters.Add("id_categoria_padre", DBNull.Value);

            }
            else if (categoria.IdCategoriaPadre > 0)
            {

                Sql += ", id_categoria_padre = @id_categoria_padre ";
                Parameters.Add("id_categoria_padre", categoria.IdCategoriaPadre);

            }

            Sql += where;

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }
    }
}
