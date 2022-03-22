using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Publicaciones;
using Touch.Core.Playlists;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Playlists
{
    public class PlaylistsRepository : SingleEntityComunRepository<Playlist>, IPlaylistsRepository
    {
        private readonly IMultimediaRepository multimediaRepository;
        private readonly IPlaylistDeSectorRepository playlistDeSectorRepository;

        public PlaylistsRepository(IConfiguration configuration, IMultimediaRepository multimediaRepository, IPlaylistDeSectorRepository playlistDeSectorRepository) : base(configuration)
        {
            this.multimediaRepository = multimediaRepository;
            this.playlistDeSectorRepository = playlistDeSectorRepository;
        }

        public override async Task<bool> Insert(Playlist entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = new string[] { "Multimedia", "Sectores", "PlaylistDeSector", "Sector" };
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
                    foreach (var objeto in entity.Multimedia)
                    {
                        objeto.IdPlaylist = result;
                        objeto.Creado = DateTime.Now;
                        if (await multimediaRepository.Insert(objeto, tran, new string[] { "Tipo", "Objeto" }))
                            continue;
                        else throw new Exception("Error al insertar objetos en la playlist");
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

        public override async Task<long> InsertAndGetId(Playlist entity, string[] columnsToIgnore = null)
        {
            columnsToIgnore = GetColumnsToIgnoreForInsert(columnsToIgnore);

            Sql = "Insert into " + GetTableName() + " (" + GetColumnsForInsert(columnsToIgnore) + ") values (" + GetParametersString(columnsToIgnore) + ");" +
                " Select SCOPE_IDENTITY()";

            long idPlaylist = 0;
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                idPlaylist = Convert.ToInt64(await ExecuteScalarQuery(Sql, GetParameters(entity, columnsToIgnore), false, tran.Connection, tran));
                foreach (var objeto in entity.Multimedia)
                {
                    objeto.IdPlaylist = idPlaylist;
                    objeto.Creado = DateTime.Now;

                    if (!await multimediaRepository.Insert(objeto, tran, new string[] { "Objeto", "Tipo", "Url" }))
                        throw new Exception();
                }

                foreach (var sector in entity.PlaylistDeSector)
                {
                    sector.IdPlaylist = idPlaylist;
                    sector.Creado = DateTime.Now;

                    if (!await playlistDeSectorRepository.Insert(sector, tran, null))
                        throw new Exception();
                }

                tran.Commit();

            }
            catch (Exception ex)
            {
                idPlaylist = 0;
                tran.Rollback();
            }

            return idPlaylist;
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


        public override async Task<IEnumerable<Playlist>> Get(string[] columnsToIgnore = null)
        {
            return await base.Get(new string[] { "Sectores", "Multimedia", "PlaylistDeSector", "Sector" });
        }

        public override async Task<IEnumerable<Playlist>> Get(string nombre, string[] columnsToIgnore = null)
        {
            return await base.Get(nombre, new string[] { "Sectores", "Multimedia", "PlaylistDeSector", "Sector" });
        }

        public override async Task<Playlist> Get(long id, string[] columnsToIgnore = null)
        {
            return await base.Get(id, new string[] { "Sectores", "Multimedia", "PlaylistDeSector", "Sector" });
        }

        public override async Task<bool> Update(Playlist playlist, string[] columnsToIgnore = null)
        {
            using SqlTransaction tran = await OpenConnectionWithTransaction();
            try
            {
                var result = false;
                var t = Task.Run(() =>
                {
                    DeleteMultimediaDePlaylist(playlist, tran);
                    DeleteSectoresDePlaylist(playlist, tran);

                    playlist.Modificado = DateTime.Now;
                    result = Update(playlist, tran, columnsToIgnore).Result;

                    if (!result)
                        throw new Exception("No se pudo actualizar la playlist");

                    InsertarMultimediaDePlaylist(playlist, playlist.Modificado.Value, tran);
                    InsertarSectoresDePlaylist(playlist, playlist.Modificado.Value, tran);
                });
                t.Wait();

                tran.Commit();
                return result;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
        }

        private bool InsertarMultimediaDePlaylist(Playlist playlist, DateTime date, SqlTransaction tran)
        {
            bool result = false;


            foreach (var objeto in playlist.Multimedia)
            {
                objeto.IdPlaylist = playlist.Id;
                objeto.Creado = DateTime.Now;

                var id = multimediaRepository.InsertAndGetId(objeto, tran, new string[] { "Objeto", "Tipo", "Url" }).Result;

                  
                if (id <= 0)
                    throw new Exception("No se pudo insertar la multimedia");
            }




            return result;
        }



        private bool InsertarSectoresDePlaylist(Playlist playlist, DateTime date, SqlTransaction tran)
        {
            bool result = false;


            foreach (var objeto in playlist.PlaylistDeSector)
            {
                objeto.IdPlaylist = playlist.Id;
                objeto.Creado = DateTime.Now;

                var id = playlistDeSectorRepository.InsertAndGetId(objeto, tran, null).Result;


                if (id <= 0)
                    throw new Exception("No se pudo insertar la multimedia");
            }




            return result;
        }

        private Task DeleteMultimediaDePlaylist(Playlist playlist, SqlTransaction tran)
        {
            var deleteResult = multimediaRepository.DeleteFromPlaylist(playlist.Id).Result;

            return Task.FromResult(1);
        }

        private Task DeleteSectoresDePlaylist(Playlist playlist, SqlTransaction tran)
        {
            var deleteResult = playlistDeSectorRepository.DeleteFromPlaylist(playlist.Id).Result;

            return Task.FromResult(1);
        }

        private static string[] GetColumnsToIgnore(string[] columnsToIgnore)
        {
            if (columnsToIgnore == null)
                columnsToIgnore = new string[] { "Modificado", "Id", "Sectores", "Multimedia", "PlaylistDeSector", "Sector" };
            else
            {
                columnsToIgnore = columnsToIgnore.Append("Modificado").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Id").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Sectores").ToArray();
                columnsToIgnore = columnsToIgnore.Append("Multimedia").ToArray();
            }

            return columnsToIgnore;
        }

        //public Publicacion EsArchivoDeAlgunaPublicacion(long id)
        //{
        //    Sql = "Select pu.id_archivo_fondo as idArchivo " + From + Where + "and pu.id_archivo_fondo = @id";
        //    Parameters = new Dictionary<string, object>() { { "id", id } };
        //    return Get<Publicacion>(Sql, Parameters).Result;
        //}
    }
}
