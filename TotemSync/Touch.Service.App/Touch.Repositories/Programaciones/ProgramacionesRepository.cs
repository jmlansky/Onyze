using Framework;
using Framework.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Programaciones;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas;

namespace Touch.Repositories.Programaciones
{
    public class ProgramacionesRepository : SingleEntityComunRepository<Programacion>, IProgramacionesRepository
    {
        private readonly IConfiguration configuration;
        private readonly IProgramacionPeriodoRepository programacionPeriodoRepository;
        private readonly IProgramacionItemRepository  programacionItemRepository;


        public ProgramacionesRepository(IConfiguration configuration, IProgramacionItemRepository programacionItemRepository,
            IProgramacionPeriodoRepository programacionPeriodoRepository ) : base(configuration)
        {
            this.configuration = configuration;
            this.programacionItemRepository = programacionItemRepository;
            this.programacionPeriodoRepository = programacionPeriodoRepository;

        }

        public override async Task<long> InsertAndGetId(Programacion entity, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

                Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                    " Select SCOPE_IDENTITY()";


                var idProgramacion = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));
                entity.Id = idProgramacion;

                var tasks = InsertarItems(entity, tran);

                tasks.AddRange(InsertarPeriodos(entity, tran).ToArray());

                Task.WaitAll(tasks.ToArray());

                tran.Commit();
                return idProgramacion;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return 0;
            }
        }

        public override async Task<bool> Update(Programacion entity, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            var result = false;
            try
            {
                DeleteItems(entity, tran);
                DeletePeriodos(entity, tran);

                entity.Modificado = DateTime.Now;
                result = Update(entity, tran, columnsToIgnore).Result;

                if (!result)
                    throw new Exception("No se pudo actualizar la gondola");

                var tasks = InsertarItems(entity, tran);

                tasks.AddRange(InsertarPeriodos(entity, tran).ToArray());

                Task.WaitAll(tasks.ToArray());

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        private void DeletePeriodos(Programacion programacion, SqlTransaction tran)
        {
            programacionPeriodoRepository.DeleteFromProgramacion(programacion.Id, tran);
        }

        private void DeleteItems(Programacion programacion, SqlTransaction tran)
        {
            programacionItemRepository.DeleteFromProgramacion(programacion.Id, tran);

        }

        private List<Task> InsertarPeriodos(Programacion programacion, SqlTransaction tran)
        {
            var tasks = new List<Task>();
            //foreach (var provincia in programacion.Provincias)
            //{
            //    provincia.IdPromocion = promo.Id;
            //    provincia.Creado = promo.Creado;
            //    tasks.Add(Task.Run(() => promocionDeProvinciasRepository.Insert(provincia, tran, new string[] { "Nombre" })));
            //}

            foreach (var periodo in programacion.Periodos)
            {
                periodo.IdProgramacion = programacion.Id;
                periodo.Creado = programacion.Creado;
                tasks.Add(Task.Run(() => programacionPeriodoRepository.Insertar(periodo, tran)));
            }

            //foreach (var region in promo.Regiones)
            //{
            //    region.IdPromocion = promo.Id;
            //    region.Creado = promo.Creado;
            //    tasks.Add(Task.Run(() => promocionDeRegionesRepository.Insert(region, tran, new string[] { "Nombre" })));
            //}
            return tasks;
        }

        private List<Task> InsertarItems(Programacion programacion, SqlTransaction tran)
        {
            var tasks = new List<Task>();
            foreach (var item in programacion.Items)
            {
                item.IdProgramacion = programacion.Id;
                item.Creado = programacion.Creado;
                tasks.Add(Task.Run(() => programacionItemRepository.Insert(item, tran, new string[] { "Nombre" })));
            }

            return tasks;
        }

       

        private string[] GetColumnsToIgnoreForInsert(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
            }

            return columnsToIgnore;
        }

        public override async Task<IEnumerable<Programacion>> Get(string[] columnsToIgnore = null)
        {
            return await base.Get(new string[] { "Items", "Periodos", "Dias", "Destinatarios", "Clientes", "Provincias", "Provincia", "Regiones", "Grupos", "Playlists", "Sponsoreos", "Zonas", "Localidades" });
        }


        public override async Task<Programacion> Get(long id, string[] columnsToIgnore = null)
        {
            return await base.Get(id, new string[] { "Items", "Periodos", "Dias", "Destinatarios", "Clientes", "Provincias", "Provincia", "Regiones", "Grupos", "Playlists", "Sponsoreos", "Zonas", "Localidades" });
        }
    }
}
