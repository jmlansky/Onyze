using Framework.Comun.Dtos.Responses;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Cliente;
using Touch.Api.Dtos.Localidades;
using Touch.Api.Dtos.Playlists;
using Touch.Api.Dtos.Programaciones;
using Touch.Api.Dtos.Provincias;
using Touch.Api.Dtos.Regiones;
using Touch.Api.Dtos.Zonas;
using Touch.Core.Programaciones;
using Touch.Core.Programaciones.Items;
using Touch.Service.Comun;
using Touch.Service.Programaciones;

namespace Touch.Api.Controllers
{
    public class ProgramacionesController : BaseController
    {
        private readonly IProgramacionesService programacionesService;


        public ProgramacionesController(IConfiguration configuration, IProgramacionesService programacionesService) : base(configuration)
        {
            this.programacionesService = programacionesService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostProgramacionDto dto)
        {



            if (dto.Destinatarios == null)
                return BadRequest("Por favor verifique los destinatarios de la promoción");


            if ((dto.Destinatarios.IdsProvincia == null || !dto.Destinatarios.IdsProvincia.Any()) &&
                  (dto.Destinatarios.IdsLocalidad == null || !dto.Destinatarios.IdsLocalidad.Any()) &&
               (dto.Destinatarios.IdsCliente == null || !dto.Destinatarios.IdsCliente.Any()) &&
               (dto.Destinatarios.IdsZona == null || !dto.Destinatarios.IdsZona.Any()) &&
               (dto.Destinatarios.IdsRegion == null || !dto.Destinatarios.IdsRegion.Any()))
                return BadRequest("Por favor seleccione algun destino para la programacion");

            if (dto.Items == null)
                return BadRequest("Por favor verifique los items de la programacion");

            if ((dto.Items.IdsPlaylist == null || !dto.Items.IdsPlaylist.Any()) &&
              (dto.Items.IdsSponsoreo == null || !dto.Items.IdsSponsoreo.Any()))
                return BadRequest("Por favor seleccione algun item para la programar");


            if (dto.Periodos == null)
                return BadRequest("Por favor verifique los períodos de la programacion");

            var programacion = await MappearDtoAProgramacion(dto);


            var result = await programacionesService.Insert(programacion);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(long id, [FromBody] PutProgramacionDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            if (dto.Destinatarios == null)
                return BadRequest("Por favor verifique los destinatarios de la promoción");


            if ((dto.Destinatarios.IdsProvincia == null || !dto.Destinatarios.IdsProvincia.Any()) &&
                  (dto.Destinatarios.IdsLocalidad == null || !dto.Destinatarios.IdsLocalidad.Any()) &&
               (dto.Destinatarios.IdsCliente == null || !dto.Destinatarios.IdsCliente.Any()) &&
               (dto.Destinatarios.IdsZona == null || !dto.Destinatarios.IdsZona.Any()) &&
               (dto.Destinatarios.IdsRegion == null || !dto.Destinatarios.IdsRegion.Any()))
                return BadRequest("Por favor seleccione algun destino para la programacion");

            if (dto.Items == null)
                return BadRequest("Por favor verifique los items de la programacion");

            if ((dto.Items.IdsPlaylist == null || !dto.Items.IdsPlaylist.Any()) &&
              (dto.Items.IdsSponsoreo == null || !dto.Items.IdsSponsoreo.Any()))
                return BadRequest("Por favor seleccione algun item para la programar");


            if (dto.Periodos == null)
                return BadRequest("Por favor verifique los períodos de la programacion");

            var programacion = await MappearDtoAProgramacion(dto);
            programacion.Id = id;

            var result = await programacionesService.Update(programacion);
            result.IdObjeto = id;
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await programacionesService.Delete(new Programacion() { Id = id });
            return StatusCode((int)result.StatusCode, result);
        }


        private async Task<Programacion> MappearDtoAProgramacion(IProgramacionDto dto)
        {
            var programacion = MapperEntidadDto.Mapper(dto, new Programacion());



            foreach (var item in dto.Periodos)
            {
                var periodo = MapperEntidadDto.Mapper(item, new ProgramacionPeriodo());
                var franjasHorarias = item.FranjasHorarias.Select(x => MapperEntidadDto.Mapper(x, new ProgramacionFranjaHoraria())).ToList();
                periodo.FranjasHorarias = franjasHorarias;
                programacion.Periodos.Add(periodo);

            }


            MapearItems(dto, programacion);

            //foreach (var item in dto.Items)
            //{
            //    if (item.Tipo.ToLower() == "publicidad")
            //        programacion.Items.Add(MapperEntidadDto.Mapper(item, new Publicacion()));
            //    else if (item.Tipo.ToLower() == "playlist")
            //        programacion.Items.Add(MapperEntidadDto.Mapper(item, new Playlist()));
            //    else if (item.Tipo.ToLower() == "promocion")
            //        programacion.Items.Add(MapperEntidadDto.Mapper(item, new Promocion()));
            //}


            ////VER mapeo de programacionParaCliente a iDestinatario
            //programacion.Destinatarios = dto.Destinatarios.Select(x => (IDestinatario)MapperEntidadDto.Mapper(x, new ProgramacionParaCliente())).ToList();
            ////var destinatario = InstancesHelper.GetInstanciaAplicar<IDestinatario>(x.Tipo);
            return programacion;
        }


        private void MapearItems(IProgramacionDto dto, Programacion programacion)
        {
            //if (dto.Destinatarios.IdsGrupo != null && dto.Destinatarios.IdsGrupo.Any())
            //    promocion.Categorias.AddRange(dto.Destinatarios.IdsGrupo.Select(x => new CategoriaDeArticulo() { Id = x }).ToList());

            //Destinos

            if (dto.Destinatarios.IdsProvincia != null && dto.Destinatarios.IdsProvincia.Any())
                programacion.Items.AddRange(dto.Destinatarios.IdsProvincia.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "provincia", IdProgramacion = programacion.Id }).ToList());

            if (dto.Destinatarios.IdsZona != null && dto.Destinatarios.IdsZona.Any())
                programacion.Items.AddRange(dto.Destinatarios.IdsZona.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "zona", IdProgramacion = programacion.Id }).ToList());

            if (dto.Destinatarios.IdsRegion != null && dto.Destinatarios.IdsRegion.Any())
                programacion.Items.AddRange(dto.Destinatarios.IdsRegion.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "region", IdProgramacion = programacion.Id }).ToList());

            if (dto.Destinatarios.IdsCliente != null && dto.Destinatarios.IdsCliente.Any())
                programacion.Items.AddRange(dto.Destinatarios.IdsCliente.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "cliente", IdProgramacion = programacion.Id }).ToList());

            if (dto.Destinatarios.IdsLocalidad != null && dto.Destinatarios.IdsLocalidad.Any())
                programacion.Items.AddRange(dto.Destinatarios.IdsLocalidad.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "localidad", IdProgramacion = programacion.Id }).ToList());


            //Origenes

            if (dto.Items.IdsPlaylist != null && dto.Items.IdsPlaylist.Any())
                programacion.Items.AddRange(dto.Items.IdsPlaylist.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "playlist", IdProgramacion = programacion.Id }).ToList());

            if (dto.Items.IdsSponsoreo != null && dto.Items.IdsSponsoreo.Any())
                programacion.Items.AddRange(dto.Items.IdsSponsoreo.Select(x => new ProgramacionItem() { IdItem = x, Tipo = "sponsoreo", IdProgramacion = programacion.Id }).ToList());



        }

        [HttpGet()]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await programacionesService.Get(pageNumber, pageSize);
            var response = new PagedResponse<GetProgramacionesDto>
            {
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                TotalRecords = result.TotalRecords
            };

            response.List = ((PagedList<Programacion>)result.PagedList).Select(x => MapearProgramacionADto(x)).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {

            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await programacionesService.Get(id);
            if (result == null || result.Id == 0)
                return NotFound("No se ha encontrado la góndola que busca.");

            var prog = MapearProgramacionADto(result);



            return Ok(prog);
        }

        private static GetProgramacionesDto MapearProgramacionADto(Programacion item)
        {
            var dtoItem = MapperEntidadDto.Mapper(item, new GetProgramacionesDto());
            dtoItem.Destinatarios = new GetDestinatarioDto();


            foreach (ProgramacionItem objeto in item.Items)
            {

                switch (objeto.Tipo.ToLower())
                {
                    case "playlist":
                        dtoItem.Items.Add(new ItemDeProgramacioGetDto() { Id = objeto.IdItem, Nombre = objeto.Nombre, Tipo = "playlist" });
                        break;
                    case "provincia":
                        var provinciaDto = MapperEntidadDto.Mapper(objeto, new ProvinciaDto());
                        provinciaDto.Id = objeto.IdItem;
                        dtoItem.Destinatarios.Provincias.Add(provinciaDto);
                        break;
                    case "region":
                        var regionDto = MapperEntidadDto.Mapper(objeto, new RegionDto());
                        regionDto.Id = objeto.IdItem;
                        dtoItem.Destinatarios.Regiones.Add(regionDto);
                        break;
                    case "localidad":
                        var localidadDto = MapperEntidadDto.Mapper(objeto, new LocalidadDto());
                        localidadDto.Id = objeto.IdItem;
                        dtoItem.Destinatarios.Localidades.Add(localidadDto);
                        break;
                    case "cliente":
                        var clienteDto = MapperEntidadDto.Mapper(objeto, new ClienteDto());
                        clienteDto.Id = objeto.IdItem;
                        dtoItem.Destinatarios.Clientes.Add(clienteDto);
                        break;
                    case "zona":
                        var zonaDto = MapperEntidadDto.Mapper(objeto, new ZonaDto());
                        zonaDto.Id = objeto.IdItem;
                        dtoItem.Destinatarios.Zonas.Add(zonaDto);
                        break;
                    default:
                        break;
                }


            }






            foreach (var periodo in item.Periodos)
            {
                var objetoDto = MapperEntidadDto.Mapper(periodo, new PeriodoDto());

                foreach (var franja in periodo.FranjasHorarias)
                {
                    var franjaDto = MapperEntidadDto.Mapper(franja, new FranjaHorariaDto());
                    objetoDto.FranjasHorarias.Add(franjaDto);
                }

                dtoItem.Periodos.Add(objetoDto);
            }

            return dtoItem;
        }

    }
}
