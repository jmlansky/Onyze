using Framework.Helpers;
using Framework.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Totem.Sync.Api.Dtos;
using Totem.Sync.Services.Contracts;
using coreTotem = Touch.Core.Totems;

namespace Totem.Sync.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TotemSyncController : BaseController
    {
        //private readonly IConfiguration configuration;
        private readonly ITotemsService totemsService;
        public TotemSyncController(IConfiguration configuration, ITotemsService totemsService) : base(configuration)
        {
            this.totemsService = totemsService;
        }


        [AllowAnonymous]
        [HttpGet("{id}/{fecha}")]
        //[Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Syncronizar(long id, string fecha)
        {
            try
            {
                if (id <= 0 || string.IsNullOrWhiteSpace(fecha))
                    return BadRequest("el id del totem tiene que ser válido y la fecha no puede ser vacía");

                CultureInfo culture = new CultureInfo("es-ES");
                var parseResult = DateTime.TryParse(fecha, culture, DateTimeStyles.None, out DateTime fechaNueva);
                if (!parseResult)
                    throw new Exception("El formato de la fecha debe ser DD-MM-YYYY");

                //DateTime fechaNueva = DateTime.Parse(fecha, culture);
                var result = await totemsService.Syncronizar(id, fechaNueva);

                //Mappear resultados
                var programacionesDto = new List<ProgramacionDto>();
                foreach (var programacion in result)
                {
                    var programacionDto = MapperEntidadDto.Mapper(programacion, new ProgramacionDto());

                    //Mappear periodos
                    if (programacion.Periodos != null && programacion.Periodos.Any())
                        programacionDto.Periodos = new List<PeriodoDto>();

                    foreach (var periodo in programacion.Periodos)
                    {
                        var periodoDto = MapperEntidadDto.Mapper(periodo, new PeriodoDto());
                        periodoDto.FranjasHorarias = periodo.FranjasHorarias.Select(x => MapperEntidadDto.Mapper(x, new FranjaHorariaDto())).ToList();
                        programacionDto.Periodos.Add(periodoDto);
                    }

                    //Mappear items programados
                    if (programacion.ItemsProgramados != null && programacion.ItemsProgramados.Any())
                        programacionDto.ItemsProgramados = new List<ItemsProgramadosDto>();
                    programacionDto.ItemsProgramados = programacion.ItemsProgramados.Select(x => new ItemsProgramadosDto() { Id = x.Id, Tipo = x.Tipo.ToString()}).ToList();

                  
                    programacionesDto.Add(programacionDto);
                }

                return Ok(programacionesDto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await totemsService.Get());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }        
    }
}
