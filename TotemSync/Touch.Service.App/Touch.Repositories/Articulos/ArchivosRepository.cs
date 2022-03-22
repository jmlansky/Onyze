using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos
{
    public class ArchivosRepository : SingleEntityComunRepository<Archivo>, IArchivosRepository
    {
        public ArchivosRepository(IConfiguration configuration) : base(configuration)
        { }

        public async Task<bool> DeleteAllFromOriginal(long id)
        {
            Sql = "update archivo set eliminado = 1, modificado = @modificado where id_archivo_original = @id_original and eliminado = 0";
            Parameters = new Dictionary<string, object>()
            {
                { "modificado", DateTime.Now},
                { "id_original", id }
            };
            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.id_articulo = @id_articulo";
            Parameters = new Dictionary<string, object>()
            {
                { "id_articulo", id }
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetFiltrados(FiltroArchivos filtros, string[] columnsToIgnore)
        {
            Parameters = new Dictionary<string, object>();
            Select = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " ";

            if (filtros.Eliminado.HasValue) Where = "WHERE ar.eliminado = @eliminado ";
            Parameters.Add("eliminado", filtros.Eliminado.HasValue ? Convert.ToInt16(filtros.Eliminado.Value) : 0);

            if (!string.IsNullOrWhiteSpace(filtros.Nombre))
            {
                Where += "and nombre like @nombre ";
                Parameters.Add("nombre", "%" + filtros.Nombre +"%");
            }
            if (!string.IsNullOrWhiteSpace(filtros.Size))
            {
                Where += "and size like @size ";
                Parameters.Add("size", "%" + filtros.Size + "%");
            }
            if (filtros.IdTipoArchivo.HasValue && filtros.IdTipoArchivo > 0)
            {
                Where += "and id_tipo = @id_tipo ";
                Parameters.Add("id_tipo", filtros.IdTipoArchivo);
            }
            if (filtros.FechaAltaInicio.HasValue)
            {
                Where += "and creado >= @creadoDesde ";
                Parameters.Add("creadoDesde", filtros.FechaAltaInicio);
            }
            if (filtros.FechaAltaFin.HasValue)
            {
                Where += "and creado <= @creadoHasta ";
                Parameters.Add("creadoHasta", filtros.FechaAltaFin);
            }

            Sql = Select + From + Where;

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.id_archivo_original = @id_archivo_original";
            Parameters = new Dictionary<string, object>()
            {
                { "id_archivo_original", idArchivo }
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetPorSize(string size)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.size = @size";
            Parameters = new Dictionary<string, object>()
            {
                { "size", size }
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.id_archivo_original = @id_archivo_original and lower(ar.size) = lower(@size)";
            Parameters = new Dictionary<string, object>()
            {
                { "id_archivo_original", idArchivo },
                { "size", size },
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.id_tipo = @id_tipo and lower(ar.size) = lower(@size)";
            Parameters = new Dictionary<string, object>()
            {
                { "id_tipo", idTipo },
                { "size", size },
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }

        public async Task<IEnumerable<Archivo>> GetPorTipo(long idTipo)
        {
            Select = "SELECT " + GetColumnsForSelect(Alias, new string[] { "Tipo", "File", "Miniaturas" }) + " ";
            Sql = Select + From + Where + " and ar.id_tipo = @id_tipo";
            Parameters = new Dictionary<string, object>()
            {
                { "id_tipo", idTipo }
            };

            return await GetListOf<Archivo>(Sql, Parameters);
        }
    }
}