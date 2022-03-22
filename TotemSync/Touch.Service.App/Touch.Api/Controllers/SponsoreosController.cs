using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using Touch.Api.Dtos;
using Touch.Api.Dtos.Sponsoreo;
using Framework.Helpers;
using Touch.Core.Articulos;
using Touch.Service.Articulos;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{  
    public class SponsoreosController : BaseController
    {
        private readonly ISponsoreadosService sponsoreoService;
        public SponsoreosController(IConfiguration configuration, ISponsoreadosService sponsoreoService): base(configuration)
        {
            this.sponsoreoService = sponsoreoService;
        }

        // Delete <SponsoreosController>/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var sponsoreo = new Sponsoreo()
            {
                Id = id,
                Modificado = DateTime.Now
            };

            var result = await sponsoreoService.Delete(sponsoreo);
            if (!result.HasErrors)
                return Ok(result);
            return BadRequest(result);
        }

        // GET<SponsoreosController>
        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await sponsoreoService.Get();
            var dto = new List<SponsoreoDto>();
            if (result.Any())
            {
                foreach (var item in result)
                {
                    var sponsoreo = new SponsoreoDto()
                    {
                        Eliminado = false,
                        FechaFin = item.FechaFin.ToString(),
                        Creado = DateTime.Now,
                        FechaInicio = item.FechaInicio.ToString(),
                        HoraFin = item.HoraFin.ToString(),
                        HoraInicio = item.HoraInicio.ToString(),
                        IdArticulo = item.IdArticulo,
                        Id = item.Id
                    };
                    dto.Add(sponsoreo);
                }
            }

            return Ok(dto);
        }

        // GET<SponsoreosController>/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id < 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await sponsoreoService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            var dto = new SponsoreoDto();
            if (result != null && result.Id > 0)
            {
                dto.Eliminado = false;
                dto.FechaFin = result.FechaFin.ToString();
                dto.Creado = DateTime.Now;
                dto.FechaInicio = result.FechaInicio.ToString();
                dto.HoraFin = result.HoraFin.ToString();
                dto.HoraInicio = result.HoraInicio.ToString();
                dto.IdArticulo = result.IdArticulo;
                dto.Id = result.Id;
                dto.IdFabricante = result.IdFabricante;
            }

            return Ok(dto);
        }

        // POST <SectorsController>
        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostSponsoreoDto dto)
        {
            try
            {
                var sponsoreo = new Sponsoreo()
                {
                    Eliminado = false,
                    FechaFin = DateTime.ParseExact(dto.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Creado = DateTime.Now,
                    FechaInicio = DateTime.ParseExact(dto.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    HoraFin = TimeSpan.Parse(dto.HoraFin),
                    HoraInicio = TimeSpan.Parse(dto.HoraInicio),
                    IdArticulo = dto.IdArticulo
                };

                var result = await sponsoreoService.Insert(sponsoreo);
                if (!result.HasErrors)
                    return Ok(result);

                return BadRequest(result);

            }
            catch (Exception ex)
            {
                return BadRequest("Error en el formato de entrada de datos: " + ex.Message.ToString());
            }
        }

        [HttpGet("Articulo/{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetSponsoreadosPorId(long id, int? pageNumber, int? pageSize)
        {
            if (pageSize <= 0 || pageNumber <= 0)
                return BadRequest("Por favor indique número de página y tamaño de página válidos");

            try
            {
                var sponsoreados = await sponsoreoService.GetFromArticulo(id);

                var pagedList = new PagedList<Sponsoreo>(sponsoreados, pageNumber ?? 1, pageSize ?? 25);
                var dto = new PagedResponse<SponsoreoDto>(pageNumber ?? 1, pageSize ?? 25, sponsoreados.Count());

                var listaDto = pagedList.Select(x => MapperEntidadDto.Mapper(x, new SponsoreoDto()));
                dto.List.AddRange(listaDto);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT <SponsoreosController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(int id, PutSponsoreoDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido.");

            var sponsoreo = new Sponsoreo()
            {
                Id = id,
                Eliminado = false,
                FechaFin = DateTime.ParseExact(dto.FechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                Creado = DateTime.Now,
                FechaInicio = DateTime.ParseExact(dto.FechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                HoraFin = TimeSpan.Parse(dto.HoraFin),
                HoraInicio = TimeSpan.Parse(dto.HoraInicio),
                IdArticulo = dto.IdArticulo,
                Modificado = DateTime.Now
            };

            var result = await sponsoreoService.Update(sponsoreo);
            if (!result.HasErrors)
                return Ok(result);

            return BadRequest(result);
        }
    }
}
