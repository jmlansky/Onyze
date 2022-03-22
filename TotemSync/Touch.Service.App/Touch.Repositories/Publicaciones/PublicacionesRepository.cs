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
    public class PublicacionesRepository : SingleEntityComunRepository<Publicacion>, IPublicacionesRepository
    {
        private readonly IObjetoAPublicitarRepository objetoRepository;
        public PublicacionesRepository(IConfiguration configuration, IObjetoAPublicitarRepository objetoRepository) : base(configuration)
        {
            this.objetoRepository = objetoRepository;
        }

        public override async Task<bool> Insert(Publicacion entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = new string[] { "Archivo", "Tipo", "ObjetosAPublicitar" };
            entity.Creado = DateTime.Now;


            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                columnsToIgnore = GetColumnsToIgnore(columnsToIgnore);

                Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + "); " +
                    "Select SCOPE_IDENTITY()";
                Parameters = GetParameters(entity, columnsToIgnore);
                var result = Convert.ToInt64(await ExecuteScalarQuery(Sql, Parameters, false, tran.Connection, tran));

                if (result > 0)
                {
                    foreach (var objeto in entity.ObjetosAPublicitar)
                    {
                        objeto.IdPantalla = result;
                        objeto.Creado = DateTime.Now;
                        if (await objetoRepository.Insert(objeto, tran, new string[] { "Tipo", "Objeto" }))
                            continue;
                        else throw new Exception("Error al insertar objetos en la publicación");
                    }
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        public override async Task<long> InsertAndGetId(Publicacion entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";

            long idPublicacion = 0;
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                idPublicacion = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));
                foreach (var objeto in entity.ObjetosAPublicitar)
                {
                    objeto.IdPantalla = idPublicacion;
                    objeto.Creado = DateTime.Now;

                    if (!await objetoRepository.Insert(objeto, tran, new string[] { "Objeto", "Tipo" }))
                        throw new Exception();
                }
                tran.Commit();

            }
            catch (Exception ex)
            {
                tran.Rollback();
            }

            return idPublicacion;
        }

        private static string[] GetColumnsToIgnoreForInsert(string[] columnsToIgnore)
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

        public async Task<bool> AsociarArchivoPublicacion(Publicacion publicacion)
        {
            Sql = "update pantalla set id_archivo_fondo = @id_archivo where id = @id and eliminado = 0";

            Parameters = new Dictionary<string, object>()
            {
                { "id_archivo", publicacion.IdArchivo },
                { "id", publicacion.Id }
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public override async Task<IEnumerable<Publicacion>> Get(string[] columnsToIgnore = null)
        {
            return await base.Get(new string[] { "Archivo", "Tipo", "ObjetosAPublicitar" });
        }

        public override async Task<IEnumerable<Publicacion>> Get(string nombre, string[] columnsToIgnore = null)
        {
            return await base.Get(nombre, new string[] { "Archivo", "ObjetosAPublicitar" });
        }

        public override async Task<Publicacion> Get(long id, string[] columnsToIgnore = null)
        {
            return await base.Get(id, new string[] { "Archivo", "Tipo", "ObjetosAPublicitar" });
        }

        public async Task<bool> DeleteObjetosPublicados(long id, long  idTipo)
        {
            return await objetoRepository.DeleteObjetosPublicados(new ObjetoAPublicar() { IdObjeto = id, IdTipo = idTipo });
        }

        private static string[] GetColumnsToIgnore(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id", "Archivo", "ObjetosAPublicitar" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Archivo").ToArray();
                columnsToIgnore = columnsToIgnore.Append("ObjetosAPublicitar").ToArray();
            }

            return columnsToIgnore;
        }

        public Publicacion EsArchivoDeAlgunaPublicacion(long id)
        {
            Sql = "Select pu.id_archivo_fondo as idArchivo " + From + Where + "and pu.id_archivo_fondo = @id";
            Parameters = new Dictionary<string, object>() { { "id", id } };
            return Get<Publicacion>(Sql, Parameters).Result;
        }
    }
}
