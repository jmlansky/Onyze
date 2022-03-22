using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Programaciones
{
    public class ProgramacionItemRepository : SingleEntityComunRepository<ProgramacionItem>, IProgramacionItemRepository
    {
        private readonly string[] columnsToIgnore = { "Tipo" };
        public ProgramacionItemRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<bool> Insert(IItem item, SqlTransaction tran, string[] columnsToIgnore = null)
        {

            var result = await base.Insert((ProgramacionItem)item, tran, columnsToIgnore);
            return result;
        }

        public async Task<IEnumerable<IItem>> GetFromProgramacion(long idProgramacion, string[] columnsToIgnore = null)
        {
            Sql = "Select " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + " and id_programacion = @id_programacion";
            return await GetListOf<ProgramacionItem>(Sql, new Dictionary<string, object> { { "id_programacion", idProgramacion } });
        }


        public async Task<bool> DeleteFromProgramacion(long idProgramacion, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_programacion = @id_programacion";
            Parameters = new Dictionary<string, object>()
            {
                { "id_programacion", idProgramacion},
                { "modificado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters, tran);
        }

    }
}
