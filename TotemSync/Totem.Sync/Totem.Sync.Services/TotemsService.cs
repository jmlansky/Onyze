using Framework.Repositories.Contracts;
using Framework.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Totem.Sync.Repositories.Contracts;
using Totem.Sync.Services.Contracts;
using Totems.Sync.Repositories.Contracts;
using Touch.Core.Clientes;
using Touch.Core.Comun;
using Touch.Core.Sucursales;
using Touch.Core.Totems;
using Touch.Core.Totems.Contracts;
using static Touch.Core.Totems.Enumerations.Enums;
using CoreTotem = Touch.Core.Totems;

namespace Totem.Sync.Services
{
    public class TotemsService : SingleEntityComunService<CoreTotem.Totem>, ITotemsService
    {
        private readonly ISectoresDelTotemRepository sectoresRepository;
        private readonly IPlaylistsRepository playlistsRepository;
        private readonly IMultimediaRepository multimediaRepository;
        private readonly ITipoMultimediaRepository tipoMultimediaRepository;
        private readonly IProgramacionesRepository programacionesRepository;
        private readonly ISingleEntityComunRepository<Sucursal> sucursalesRepository;
        private readonly IItemsProgramacionRepository itemsProgramacionRepository;
        private readonly IPeriodosRepository periodosRepository;
        private readonly IFranjasHorariasRepository franjasHorariasRepository;


        private readonly List<IProgramacionItemService<IProgramacionItem>> programacionItemServices;

        public TotemsService(ITotemsRepository totemsRepository,
            ISectoresDelTotemRepository sectoresRepository,
            IPlaylistsRepository playlistsRepository,
            IMultimediaRepository multimediaRepository,
            ITipoMultimediaRepository tipoMultimediaRepository,
            IProgramacionesRepository programacionesRepository,
            ISingleEntityComunRepository<Sucursal> sucursalesRepository,
            IItemsProgramacionRepository itemsProgramacionRepository,
            IEnumerable<IProgramacionItemService<IProgramacionItem>> itemsServices,
            IPeriodosRepository periodosRepository,
            IFranjasHorariasRepository franjasHorariasRepository
            ) : base(totemsRepository)
        {
            this.sectoresRepository = sectoresRepository;
            this.playlistsRepository = playlistsRepository;
            this.multimediaRepository = multimediaRepository;
            this.tipoMultimediaRepository = tipoMultimediaRepository;
            this.programacionesRepository = programacionesRepository;
            this.sucursalesRepository = sucursalesRepository;
            this.itemsProgramacionRepository = itemsProgramacionRepository;
            this.periodosRepository = periodosRepository;
            this.franjasHorariasRepository = franjasHorariasRepository;

            programacionItemServices = itemsServices.ToList();
        }

        public async Task<List<Programacion>> Syncronizar(long id, DateTime fechaNueva)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            var totem = await base.Get(id);
            Parallel.Invoke(() =>
            {
                totem.IdSectores = sectoresRepository.GetPorIdTotem(id).Result.Select(x => x.Id).ToList();
                totem.Sucursal = sucursalesRepository.Get(totem.IdSucursal).Result;
            });

            var listaDeProgramaciones = ObtenerProgramaciones(totem, fechaNueva);

            stopwatch.Stop();
            Console.WriteLine("Elapsed Time is {0} ms", stopwatch.ElapsedMilliseconds);

            return listaDeProgramaciones.ToList();
        }

        private IEnumerable<Programacion> ObtenerProgramaciones(CoreTotem.Totem totem, DateTime fecha)
        {
            var playlists = new List<Playlist>();
            var itemsProgramacion = new List<ProgramacionItem>();
            var tiposMultimedia = new List<TipoMultimedia>();
            var listaDeProgramaciones = new List<Programacion>();

            Parallel.Invoke(() =>
            {
                itemsProgramacion = itemsProgramacionRepository.Get().Result.ToList();
                playlists = ObtenerTodasLasPlaylists(totem.IdSectores);
                tiposMultimedia = (List<TipoMultimedia>)tipoMultimediaRepository.Get().Result;

                var idsProgramaciones = ObtenerIdsProgramaciones(totem, playlists, itemsProgramacion);
                listaDeProgramaciones = programacionesRepository.Get(idsProgramaciones.Distinct().ToList()).Result.ToList();
            });

            ObtenerPeriodos(listaDeProgramaciones, fecha);

            listaDeProgramaciones = listaDeProgramaciones.Where(x => x.Periodos != null && x.Periodos.Any()).ToList();
            if (!listaDeProgramaciones.Any())
                return listaDeProgramaciones;

            Parallel.ForEach(listaDeProgramaciones, programacion =>
            {
                var playlistsFiltradas = itemsProgramacion.Where(x => x.Tipo == "playlist" && x.IdProgramacion == programacion.Id);

                Parallel.ForEach(playlistsFiltradas.Where(x => x.IdItem != 0), playlist =>
                {
                    var itemsProgramados = multimediaRepository.GetPorIdPlaylist(playlist.IdItem).Result;
                    foreach (var itemProgramado in itemsProgramados)
                    {
                        var tipo = tiposMultimedia.FirstOrDefault(x => x.Id == itemProgramado.IdTipo).Nombre;
                        var enumValues = Enum.GetValues(typeof(TipoObjetoPlaylistEnum));
                        foreach (var item in enumValues)
                        {
                            if (tipo == item.ToString())
                            {
                                itemProgramado.Tipo = (TipoObjetoPlaylistEnum)item;
                                break;
                            }
                        }
                    }
                    programacion.ItemsProgramados = itemsProgramados;
                });
            });


            return listaDeProgramaciones;
        }

        private List<long> ObtenerIdsProgramaciones(CoreTotem.Totem totem, List<Playlist> playlists, List<ProgramacionItem> itemsProgramacion)
        {
            var idsProgramaciones = new List<long>();
            if (playlists != null && playlists.Any())
            {
                var itemsPlaylist = itemsProgramacion.Where(x => x.Tipo == "playlist");
                var lista = from item in itemsPlaylist
                            join playlist in itemsProgramacion on item.IdItem equals playlist.Id
                            select item.IdProgramacion;
                idsProgramaciones.AddRange(lista.Distinct());
            }

            if (totem.Sucursal != null && totem.Sucursal.IdZona != null && totem.Sucursal.IdZona > 0)
            {
                var promociones = itemsProgramacion.Where(x => x.Tipo == "zona" && x.IdItem == totem.Sucursal.IdZona).Select(x => x.Id);
                idsProgramaciones.AddRange(promociones);
            }

            if (totem.Sucursal != null && totem.Sucursal.IdLocalidad != null && totem.Sucursal.IdLocalidad > 0)
            {
                var promociones = itemsProgramacion.Where(x => x.Tipo == "localidad" && x.IdItem == totem.Sucursal.IdLocalidad).Select(x => x.Id);
                idsProgramaciones.AddRange(promociones);
            }

            if (totem.Sucursal != null && totem.Sucursal.IdProvincia != null && totem.Sucursal.IdProvincia > 0)
            {
                var promociones = itemsProgramacion.Where(x => x.Tipo == "provincia" && x.IdItem == totem.Sucursal.IdProvincia).Select(x => x.Id);
                idsProgramaciones.AddRange(promociones);
            }

            if (totem.Sucursal != null && totem.Sucursal.IdRegion != null && totem.Sucursal.IdRegion > 0)
            {
                var promociones = itemsProgramacion.Where(x => x.Tipo == "region" && x.IdItem == totem.Sucursal.IdRegion).Select(x => x.Id);
                idsProgramaciones.AddRange(promociones);
            }

            return idsProgramaciones;
        }

        private void ObtenerPeriodos(IEnumerable<Programacion> listaDeProgramaciones, DateTime fecha)
        {
            if (listaDeProgramaciones != null && listaDeProgramaciones.Any())
            {
                Parallel.ForEach(listaDeProgramaciones, programacion =>
                {
                    var periodos = (periodosRepository.GetPorIdProgramacion(programacion.Id)).Result.ToList().Where(x => x.FechaFin > fecha);
                    foreach (var periodo in periodos)
                        periodo.FranjasHorarias = franjasHorariasRepository.GetPorPeriodo(periodo.Id).Result.ToList();

                    programacion.Periodos = periodos.ToList();
                });
            }
        }

        /// <summary>
        /// Obtener todas las playlists, con y sin sector asociado
        /// </summary>
        /// <returns>Lista de todas las playlists que tengan o no, un sector asociado</returns>
        private List<Playlist> ObtenerTodasLasPlaylists(IEnumerable<long> idsSectores)
        {
            var playlists = new List<Playlist>();
            Parallel.Invoke(() =>
            {
                Parallel.ForEach(idsSectores, idsSector =>
                {
                    playlists.AddRange(playlistsRepository.GetPorIdSector(idsSector).Result.ToList());
                });

                playlists.AddRange(playlistsRepository.GetPlaylistsSinSector().Result.ToList());
            });

            return playlists;
        }
    }
}
