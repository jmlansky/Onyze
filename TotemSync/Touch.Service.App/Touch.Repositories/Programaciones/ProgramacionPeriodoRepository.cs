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
    public class ProgramacionPeriodoRepository : SingleEntityComunRepository<ProgramacionPeriodo>, IProgramacionPeriodoRepository
    {
        private readonly IProgramacionFranjaHorariaRepository programacionFranjaHorariaRepository;

        private readonly string[] columnsToIgnore = { "Tipo" };
        public ProgramacionPeriodoRepository(IConfiguration configuration,
            IProgramacionFranjaHorariaRepository programacionFranjaHorariaRepository) : base(configuration)
        {
            this.programacionFranjaHorariaRepository = programacionFranjaHorariaRepository;
        }

        public async Task<bool> Insertar(ProgramacionPeriodo periodo, SqlTransaction tran)
        {


            string[] columnsToIgnore = new string[] { "Modificado", "Id","FranjasHorarias" };

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";



            var idPeriodo = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(periodo, columnsToIgnore), false, tran.Connection, tran));

            periodo.Id = idPeriodo;

            var tasks = InsertarFranjasHorarias(periodo, tran);
           

            Task.WaitAll(tasks.ToArray());


            return true;
        }

        private List<Task> InsertarFranjasHorarias(ProgramacionPeriodo entity, SqlTransaction tran)
        {
            var tasks = new List<Task>();

            foreach (var franjaHoraria in entity.FranjasHorarias)
            {
                franjaHoraria.IdProgramacionesPeriodo = entity.Id;
                franjaHoraria.Creado = entity.Creado;
                tasks.Add(Task.Run(() => programacionFranjaHorariaRepository.Insert(franjaHoraria, tran)));
            }

            return tasks;
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

        public async Task<IEnumerable<ProgramacionPeriodo>> GetFromProgramacion(long idProgramacion, string[] columnsToIgnore = null)
        {
            Sql = "Select " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + " and id_programacion = @id_programacion";
            return await GetListOf<ProgramacionPeriodo>(Sql, new Dictionary<string, object> { { "id_programacion", idProgramacion } });
        }
    }
}
