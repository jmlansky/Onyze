using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{

    public class PlaylistDeSectorRepository :  SingleEntityComunRepository<PlaylistDeSector>, IPlaylistDeSectorRepository
    {

        public PlaylistDeSectorRepository(IConfiguration configuration) : base(configuration)
        { }

        public async Task<bool> DeleteFromPlaylist(long idPromocion, SqlTransaction tran, string[] columnsToIgnore = null)
        {
            Sql = "UPDATE " + GetTableName() + " SET eliminado = 1, modificado = @modificado WHERE eliminado = 0 and id = @id";
            Parameters = new Dictionary<string, object>()
            {
                { "id", idPromocion},
                { "modificado", DateTime.Now}
            };

            return await ExecuteInsertOrUpdate(Sql, Parameters, tran);
        }

        public async Task<IEnumerable<PlaylistDeSector>> GetFromPlaylist(long idPlaylist, string[] columnsToIgnore = null)
        {
            Sql = "Select " + GetColumnsForSelect(Alias, columnsToIgnore) + " " + From + Where + " and id_playlist = @id_playlist";
            Parameters = new Dictionary<string, object>()
            {
                { "id_playlist", idPlaylist }
            };

            return await GetListOf<PlaylistDeSector>(Sql, Parameters);
        }

        public async Task<bool> DeleteFromPlaylist(long id)
        {
            var result = false;
            var t = Task.Run(() =>
            {
                using SqlTransaction tran = OpenConnectionWithTransaction().Result;
                try
                {
                    result = DeleteSectoresFromPlaylist(id, tran);

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
                    result = DeleteSectoresFromPlaylist(id, tran);
                });
                t.Wait();
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool DeleteSectoresFromPlaylist(long id, SqlTransaction tran)
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


    }

}
