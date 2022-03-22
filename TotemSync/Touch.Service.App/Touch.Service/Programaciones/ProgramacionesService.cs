using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Programaciones;
using Touch.Repositories.Clientes.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Playlists;
using Touch.Repositories.Programaciones;
using Touch.Service.Comun;
using Touch.Service.Programaciones;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Core.Programaciones.Items
{
    public class ProgramacionesService : SingleEntityComunService<Programacion>, IProgramacionesService
    {
        private readonly IProgramacionesRepository programacionesRepository;
        private readonly IZonasRepository zonasRepository;
        private readonly ILocalidadesRepository localidadesRepository;
        private readonly IClientesRepository clientesRepository;
        private readonly IProgramacionPeriodoRepository programacionPeriodoRepository;
        private readonly IRegionesRepository regionesRepository;
        private readonly IProgramacionItemRepository programacionItemRepository;
        private readonly IPlaylistsRepository playlistsRepository;

        private readonly IProgramacionFranjaHorariaRepository programacionFranjaHorariaRepository;

        private readonly IProvinciasRepository provinciasRepository;

        public string[] columnsToIgnore = { "Items", "Periodos", "Dias", "Destinatarios", "Clientes", "Provincias", "Regiones", "Grupos", "Playlists", "Sponsoreos", "Zonas", "Localidades" };

        public ProgramacionesService(IProgramacionesRepository programacionesRepository,
             IProgramacionItemRepository programacionItemRepository,
            IProvinciasRepository provinciasRepository,
            IZonasRepository zonasRepository,
            IRegionesRepository regionesRepository,
            IProgramacionPeriodoRepository programacionPeriodoRepository,
            IProgramacionFranjaHorariaRepository programacionFranjaHorariaRepository,
            ILocalidadesRepository localidadesRepository,
            IClientesRepository clientesRepository,
            IPlaylistsRepository playlistsRepository
            ) : base(programacionesRepository)
        {
            this.programacionesRepository = programacionesRepository;
            this.provinciasRepository = provinciasRepository;
            this.zonasRepository = zonasRepository;
            this.regionesRepository = regionesRepository;
            this.programacionPeriodoRepository = programacionPeriodoRepository;
            this.programacionFranjaHorariaRepository = programacionFranjaHorariaRepository;
            this.localidadesRepository = localidadesRepository;
            this.clientesRepository = clientesRepository;
            this.playlistsRepository = playlistsRepository;
            this.programacionItemRepository = programacionItemRepository;
        }

        public override Task<IEnumerable<Programacion>> Get()
        {
            var programaciones = programacionesRepository.Get().Result;

            foreach (var programacion in programaciones)
            {
                CompletarDatosDeProgramacion(programacion);

            }


            return Task.FromResult(programaciones);
        }

        public override Task<Programacion> Get(long id)
        {
            var programacion = programacionesRepository.Get(id).Result;

            CompletarDatosDeProgramacion(programacion);




            return Task.FromResult(programacion);
        }


        private void CompletarDatosDeProgramacion(Programacion programacion)
        {
            //promocion.Tipo = tipoPromocionRepo.Get(promocion.IdTipo).Result;

            //var tipoDeItem = tiposItemRepository.Get(promocion.IdTipoItem).Result;
            //promocion.TipoItem = tipoDeItem.Nombre;

            ObtenerItemsDeLaProgramacion(programacion);

            ObtenerPeriodosDeLaProgramacion(programacion);
        }

        private void ObtenerItemsDeLaProgramacion(Programacion programacion)
        {
            var items = programacionItemRepository.GetFromProgramacion(programacion.Id).Result.ToList();
            if (items.Any())
            {
                var playlists = playlistsRepository.Get().Result;
                var provincias = provinciasRepository.Get().Result;
                var zonas = zonasRepository.Get().Result;
                var regiones = regionesRepository.Get().Result;
                var localidades = localidadesRepository.Get().Result;
                var clientes = clientesRepository.Get().Result;

                foreach (var item in items)
                {
                    if (item.Tipo.ToLower() == "playlist")
                    {
                        var playlist = playlistsRepository.Get(item.IdItem).Result;
                        programacion.Items.Add(new ProgramacionItem { IdItem = playlist.Id, Id = item.Id, Nombre = playlist.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }

                    if (item.Tipo.ToLower() == "provincia")
                    {
                        var provi = provincias.FirstOrDefault(x => x.Id == item.IdItem);
                        programacion.Items.Add(new ProgramacionItem { IdItem = provi.Id, Id = item.Id, Nombre = provi.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }

                    if (item.Tipo.ToLower() == "zona")
                    {
                        var zona = zonas.FirstOrDefault(x => x.Id == item.IdItem);
                        programacion.Items.Add(new ProgramacionItem { IdItem = zona.Id, Id = item.Id, Nombre = zona.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }

                    if (item.Tipo.ToLower() == "region")
                    {
                        var region = regiones.FirstOrDefault(x => x.Id == item.IdItem);
                        programacion.Items.Add(new ProgramacionItem { IdItem = region.Id, Id = item.Id, Nombre = region.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }

                    if (item.Tipo.ToLower() == "localidad")
                    {
                        var localidad = localidades.FirstOrDefault(x => x.Id == item.IdItem);
                        programacion.Items.Add(new ProgramacionItem { IdItem = localidad.Id, Id = item.Id, Nombre = localidad.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }

                    if (item.Tipo.ToLower() == "cliente")
                    {
                        var cliente = clientes.FirstOrDefault(x => x.Id == item.IdItem);
                        programacion.Items.Add(new ProgramacionItem { IdItem = cliente.Id, Id = item.Id, Nombre = cliente.Nombre, IdProgramacion = item.IdProgramacion, Tipo = item.Tipo.ToLower() });
                    }
                }
            }


        }



        private void ObtenerPeriodosDeLaProgramacion(Programacion programacion)
        {
            programacion.Periodos = programacionPeriodoRepository.GetFromProgramacion(programacion.Id, new string[] { "FranjasHorarias" }).Result.ToList();
            if (programacion.Periodos.Any())
            {
                var franjas = programacionFranjaHorariaRepository.Get().Result;
                foreach (var periodo in programacion.Periodos)
                {

                    var franja = franjas.Where(x => x.IdProgramacionesPeriodo == periodo.Id);
                    if (franja != null)
                        periodo.FranjasHorarias.AddRange(franja);
                }
            }
        }






        //private void ObtenerRegionesDeLaProgramacion(Programacion programacion)
        //{
        //    programacion.Regiones = programacionParaRegionRepository.GetFromProgramacion(programacion.Id).Result.ToList();
        //    if (programacion.Regiones.Any())
        //    {
        //        var regiones = regionesRepository.Get().Result;
        //        foreach (var region in programacion.Regiones)
        //        {
        //            var reg = regiones.FirstOrDefault(x => x.Id == region.IdRegion);
        //            if (reg != null)
        //            {
        //                region.Id = reg.Id;
        //                region.Nombre = reg.Nombre;
        //            }
        //        }
        //    }
        //}

        public override async Task<ServiceResult> Insert(Programacion programacion)
        {
            var result = GetServiceResult(ServiceMethod.Insert, "Programacion", true);


            programacion.Creado = DateTime.Now;

            result.IdObjeto = await programacionesRepository.InsertAndGetId(programacion, columnsToIgnore);

            if (result.IdObjeto <= 0)
                return GetServiceResult(ServiceMethod.Insert, "Programacion", false);






            return result;
        }



        public override async Task<ServiceResult> Update(Programacion programacion)
        {
            var result = GetServiceResult(ServiceMethod.Update, "Programacion", true);


            programacion.Creado = DateTime.Now;



            return GetServiceResult(ServiceMethod.Update, "Programacion", await programacionesRepository.Update(programacion, columnsToIgnore));








            return result;
        }
    }
}