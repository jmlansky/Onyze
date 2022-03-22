using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Publicaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Publicaciones
{
    public class ObjetoAPublicitarRepository : SingleEntityComunRepository<ObjetoAPublicar>, IObjetoAPublicitarRepository
    {
        public ObjetoAPublicitarRepository(IConfiguration configuration): base(configuration)
        {
        }

        public override async Task<bool> Delete(ObjetoAPublicar entity, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_objeto = @id_objeto " +
                "and id_pantalla = @id_pantalla and id_tipo = @id_tipo";
            Parameters = new Dictionary<string, object>();
            GetParameter(Parameters, entity, "id");
            GetParameter(Parameters, entity, "modificado");
            GetParameter(Parameters, entity, "idtipo");
            GetParameter(Parameters, entity, "idpantalla");
            GetParameter(Parameters, entity, "idobjeto");

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> DeleteObjetosPublicados(ObjetoAPublicar entity)
        {
            Sql = "UPDATE objeto_publicitado SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_objeto = @id_objeto and id_tipo = @id_tipo";
            Parameters = new Dictionary<string, object>()
            {
                { "id_objeto", entity.IdObjeto},
                { "id_tipo", entity.IdTipo},
                { "modificado", DateTime.Now},
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<IEnumerable<ObjetoAPublicar>> GetFromPublicacion(long id, string[] columnsToIgnore = null)
        {
            Sql = "SELECT "+ GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + "and id_pantalla = @id_pantalla";
            return await GetListOf<ObjetoAPublicar>(Sql, new Dictionary<string, object>() { { "id_pantalla", id } });
        }

        public async Task<IEnumerable<ObjetoAPublicar>> GetPorIdObjetoYTipo(long idObjeto, long idTipo, string[] columnsToIgnore = null)
        {
            Sql = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + "and id_objeto = @id_objeto and id_tipo = @id_tipo";
            return await GetListOf<ObjetoAPublicar>(Sql, new Dictionary<string, object>() 
            { 
                { "id_objeto", idObjeto },
                { "id_tipo", idTipo }
            });
        }

        public async Task<bool> Insert(ObjetoAPublicar entity, SqlTransaction transaction, string[] columnsToIgnore = null )
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id"};
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }
            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ")";
            Parameters = GetParameters(entity, columnsToIgnore);
            return await ExecuteInsertOrUpdate(Sql, Parameters, transaction);
        }
    }
}
