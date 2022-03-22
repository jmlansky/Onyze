using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public class MultimediaRepository : SingleEntityComunRepository<Multimedia>, IMultimediaRepository
    {
        public MultimediaRepository(IConfiguration configuration): base(configuration)
        {
        }

        public override async Task<bool> Delete(Multimedia entity, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_objeto = @id_objeto " +
                "and id_playlist = @id_playlist and id_tipo = @id_tipo";
            Parameters = new Dictionary<string, object>();
            GetParameter(Parameters, entity, "id");
            GetParameter(Parameters, entity, "modificado");
            GetParameter(Parameters, entity, "id_tipo");
            GetParameter(Parameters, entity, "id_playlist");
            GetParameter(Parameters, entity, "id_objeto");

            return await ExecuteInsertOrUpdate(Sql, Parameters);
        }

        public async Task<bool> DeleteFromPlaylist(long id)
        {
            var result = false;
            var t = Task.Run(() =>
            {
                using SqlTransaction tran = OpenConnectionWithTransaction().Result;
                try
                {
                    result = DeleteMultimediaFromPlaylist(id, tran);

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    id = 0;
                }
            });
            t.Wait();
            return result;
        }
        public async Task<bool> DeleteFromPlaylist(long id, SqlTransaction tran)
        {
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    result = DeleteMultimediaFromPlaylist(id, tran);
                });
                t.Wait();
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool DeleteMultimediaFromPlaylist(long id, SqlTransaction tran)
        {
            bool result;

            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id_playlist = @id";
            Parameters = new Dictionary<string, object>() {
                { "id", id },
                { "modificado", DateTime.Now}
            };

            result = ExecuteInsertOrUpdate(Sql, Parameters).Result;
            //if (result)
            //    result = decoracionRepository.DeleteFromEstante(id, tran).Result;

            //if (result)
            //    result = articulosPorEstanteRepository.DeleteAllArticulosDelEstante(id, tran).Result;

            return result;
        }

        public async Task<IEnumerable<Multimedia>> GetFromPlaylist(long id, string[] columnsToIgnore = null)
        {
            Sql = "SELECT "+ GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + "and id_playlist = @id_playlist";
            return await GetListOf<Multimedia>(Sql, new Dictionary<string, object>() { { "id_playlist", id } });
        }

        public async Task<IEnumerable<Multimedia>> GetPorIdObjetoYTipo(long idObjeto, long idTipo, string[] columnsToIgnore = null)
        {
            Sql = "SELECT " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + "and id_objeto = @id_objeto and id_tipo = @id_tipo";
            return await GetListOf<Multimedia>(Sql, new Dictionary<string, object>() 
            { 
                { "id_objeto", idObjeto },
                { "id_tipo", idTipo }
            });
        }

        public async Task<bool> Insert(Multimedia entity, SqlTransaction transaction, string[] columnsToIgnore = null )
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
