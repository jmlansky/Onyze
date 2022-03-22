using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Touch.Api.Dtos;
using Touch.Api.Dtos.Promociones;
using Framework.Helpers;
using Touch.Core.Promociones;
using Touch.Service.Comun;
using Touch.Service.Promociones;
using static Touch.Core.Invariants.InvariantObjects;
using System.Collections.Generic;
using PagedList;
using Framework.Comun.Dtos.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace Touch.Api.Controllers
{
    public class PromocionesController : BaseController
    {
        #region Constructor y propiedades

        private readonly IPromocionesService promocionesService;
        private readonly ISingleEntityComunService<TipoItemPromocion> tipoItemServicePromocionService;
        public PromocionesController(IConfiguration configuration, IPromocionesService promocionesService, 
            ISingleEntityComunService<TipoItemPromocion> tipoItemServicePromocionService): base(configuration)
        {
            this.promocionesService = promocionesService;
            this.tipoItemServicePromocionService = tipoItemServicePromocionService;
        }

        #endregion


        [HttpPost]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Post([FromBody] PostPromocionDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Por favor seleccione algún artículo para promocionar");

            if (dto.Destinatarios == null)
                return BadRequest("Por favor verifique los destinatarios de la promoción");

            if ((dto.Destinatarios.IdsGrupo == null || !dto.Destinatarios.IdsGrupo.Any()) &&
                (dto.Destinatarios.IdsProvincia == null || !dto.Destinatarios.IdsProvincia.Any()) &&
                (dto.Destinatarios.IdsCliente == null || !dto.Destinatarios.IdsCliente.Any()) &&
                (dto.Destinatarios.IdsRegion == null || !dto.Destinatarios.IdsRegion.Any()))
                return BadRequest("Por favor seleccione algun beneficiado para la promoción");

            var promocion = await MappearDtoAPromocion(dto);

            var result = await promocionesService.Insert(promocion);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Put(long id, [FromBody] PutPromocionDto dto)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            if (dto.Items == null || !dto.Items.Any())
                return BadRequest("Por favor seleccione algún artículo para promocionar");

            if (dto.Destinatarios == null)
                return BadRequest("Por favor verifique los destinatarios de la promoción");

            if ((dto.Destinatarios.IdsGrupo == null || !dto.Destinatarios.IdsGrupo.Any()) &&
                (dto.Destinatarios.IdsProvincia == null || !dto.Destinatarios.IdsProvincia.Any()) &&
                (dto.Destinatarios.IdsCliente == null || !dto.Destinatarios.IdsCliente.Any()) &&
                (dto.Destinatarios.IdsRegion == null || !dto.Destinatarios.IdsRegion.Any()))
                return BadRequest("Por favor seleccione algun beneficiado para la promoción");

            var promocion = await MappearDtoAPromocion(dto);
            promocion.Id = id;

            var result = await promocionesService.Update(promocion);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await promocionesService.Get(id);
            if (result.Id <= 0)
                return NotFound();

            GetPromocionDto promocionDto = MapearPromocion(result);

            return Ok(promocionDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await promocionesService.Get();
            var dto = new List<GetPromocionDto>();
            if (!result.Any())
                return Ok(dto);

            dto = result.Select(x => MapearPromocion(x)).ToList();

            var pagedList = new PagedList<GetPromocionDto>(dto, pageNumber ?? 1, pageSize ?? 25);
            var response = new PagedResponse<GetPromocionDto>(pageNumber ?? 1, pageSize ?? 25, result.Count())
            {
                List = pagedList.Select(x => x).ToList()
            };

            return Ok(response);
        }

        [HttpPost("Filtros")]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> GetPorDestinatarios(int? pageNumber, int? pageSize, [FromBody] FiltrosPromocionesDto dto)
        {
            if (dto == null)
                return BadRequest("El dto no puede estar vacío");

            var filtros = MapearDtoAFiltros(dto);
            var result = await promocionesService.GetPorFiltro(filtros, pageNumber ?? 1, pageSize ?? 25);

            var response = new PagedResponse<GetPromocionDto>(pageNumber ?? 1, pageSize ?? 25, result.Count());
            foreach (var promocion in result)
            {
                var promocionDto = MapperEntidadDto.Mapper(promocion, new GetPromocionDto());
                promocionDto.Clientes = promocion.Clientes.Select(x => MapperEntidadDto.Mapper(x, new GetClienteDePromocionDto())).ToList();
                promocionDto.Provincias = promocion.Provincias.Select(x => MapperEntidadDto.Mapper(x, new GetProvinciaDePromocionDto())).ToList();
                promocionDto.Regiones = promocion.Regiones.Select(x => MapperEntidadDto.Mapper(x, new GetRegionDePromocionDto())).ToList();
                promocionDto.Grupos = promocion.Provincias.Select(x => MapperEntidadDto.Mapper(x, new GetGrupoDeClientesDto())).ToList();

                promocionDto.TipoDeItem = new TipoItemDto() { Id = promocion.IdTipoItem, Nombre = promocion.TipoItem };
                promocionDto.TipoDePromocion = MapperEntidadDto.Mapper(promocion.Tipo, new TipoPromocionDto());

                foreach (var item in promocion.ItemsDePromocion)
                    promocionDto.Items.Add(MapperEntidadDto.Mapper(item, new GetDetallePromocionDto()));

                //promocionDto.Items = promocion.ItemsDePromocion.Select(x => MapperEntidadDto.Mapper(x, new GetDetallePromocionDto())).ToList();

                response.List.Add(promocionDto);
            }

            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Super")]
        public async Task<IActionResult> Delete(long id)
        {
            if (id <= 0)
                return BadRequest("Por favor ingrese un id válido");

            var result = await promocionesService.Delete(new Promocion() { Id = id });

            return StatusCode((int)result.StatusCode, result);
        }

        #region Métodos Privados

        private GetPromocionDto MapearPromocion(Promocion result)
        {
            var promocionDto = MapperEntidadDto.Mapper(result, new GetPromocionDto());

            MapearTipoDePromocion(result, promocionDto);
            MapearItemsDePromocion(result, promocionDto);
            MapearDestinatariosDePromocion(result, promocionDto);
            return promocionDto;
        }

        private void MapearTipoDePromocion(Promocion result, GetPromocionDto promocionDto)
        {
            promocionDto.TipoDeItem = new TipoItemDto() { Id = result.IdTipoItem, Nombre = result.TipoItem };
            promocionDto.TipoDePromocion = MapperEntidadDto.Mapper(result.Tipo, new TipoPromocionDto());
        }

        private void MapearDestinatariosDePromocion(Promocion result, GetPromocionDto promocionDto)
        {
            promocionDto.Clientes = result.Clientes.Select(x => MapperEntidadDto.Mapper(x, new GetClienteDePromocionDto())).ToList();
            promocionDto.Provincias = result.Provincias.Select(x => MapperEntidadDto.Mapper(x, new GetProvinciaDePromocionDto())).ToList();
            promocionDto.Regiones = result.Regiones.Select(x => MapperEntidadDto.Mapper(x, new GetRegionDePromocionDto())).ToList();
            promocionDto.Grupos = result.Grupos.Select(x => MapperEntidadDto.Mapper(x, new GetGrupoDeClientesDto())).ToList();
        }

        private void MapearItemsDePromocion(Promocion result, GetPromocionDto promocionDto)
        {
            if (result.ItemsDePromocion.Any())
            {
                if (result.ItemsDePromocion.FirstOrDefault().GetType() == typeof(DetallePromocion))
                {
                    foreach (var item in result.ItemsDePromocion)
                    {
                        var dto = MapperEntidadDto.Mapper((DetallePromocion)item, new GetDetallePromocionDto());
                        dto.Id = ((DetallePromocion)item).IdArticulo;
                        promocionDto.Items.Add(dto);
                    }
                }

                if (result.ItemsDePromocion.FirstOrDefault().GetType() == typeof(PromocionDeCategoria))
                {
                    foreach (var item in result.ItemsDePromocion)
                    {
                        var dto = MapperEntidadDto.Mapper((PromocionDeCategoria)item, new GetCategoriaDePromocionDto());
                        dto.Id = ((PromocionDeCategoria)item).IdCategoria;
                        promocionDto.Items.Add(dto);
                    }
                }

                if (result.ItemsDePromocion.FirstOrDefault().GetType() == typeof(PromocionDeFabricantes))
                    promocionDto.Items = result.ItemsDePromocion
                        .Select(x => new GetFabricanteDePromocionDto() { Nombre = ((PromocionDeFabricantes)x).Nombre }).ToList<IGetItemDePromocionDto>();
            }
        }

        private FiltrosDePromocion MapearDtoAFiltros(FiltrosPromocionesDto dto)
        {
            var filtros = new FiltrosDePromocion()
            {
                Id = dto.Id,
                IdTipo = dto.IdTipo
            };

            if (dto.Articulos != null && dto.Articulos.Any())
                dto.Articulos.Remove(dto.Articulos.FirstOrDefault(x => x.IdArticulo == 0));

            if (dto.Articulos != null && dto.Articulos.Any())
                filtros.Articulos = dto.Articulos.Select(x => MapperEntidadDto.Mapper(x, new DetallePromocion())).ToList();
            MapearFiltrosDeDestinatarios(dto, filtros);

            return filtros;
        }

        private static void MapearFiltrosDeDestinatarios(FiltrosPromocionesDto dto, FiltrosDePromocion filtros)
        {
            if (dto.Destinatarios.IdsProvincia != null && dto.Destinatarios.IdsProvincia.Any())
            {
                if (dto.Destinatarios.IdsProvincia.Count() == 1 && dto.Destinatarios.IdsProvincia.FirstOrDefault() == 0)
                    dto.Destinatarios.IdsProvincia.RemoveAt(0);

                filtros.Provincias = dto.Destinatarios.IdsProvincia.Select(x => new PromocionDeProvincia() { IdProvincia = x }).ToList();
            }

            if (dto.Destinatarios.IdsCliente != null && dto.Destinatarios.IdsCliente.Any())
            {
                if (dto.Destinatarios.IdsCliente.Count() == 1 && dto.Destinatarios.IdsCliente.FirstOrDefault() == 0)
                    dto.Destinatarios.IdsCliente.RemoveAt(0);

                filtros.Clientes = dto.Destinatarios.IdsCliente.Select(x => new PromocionDeCliente() { IdCliente = x }).ToList();
            }

            if (dto.Destinatarios.IdsRegion != null && dto.Destinatarios.IdsRegion.Any())
            {
                if (dto.Destinatarios.IdsRegion.Count() == 1 && dto.Destinatarios.IdsRegion.FirstOrDefault() == 0)
                    dto.Destinatarios.IdsRegion.RemoveAt(0);

                filtros.Regiones = dto.Destinatarios.IdsRegion.Select(x => new PromocionDeRegiones() { IdRegion = x }).ToList();
            }

            if (dto.Destinatarios.IdsGrupo != null && dto.Destinatarios.IdsGrupo.Any())
            {
                if (dto.Destinatarios.IdsGrupo.Count() == 1 && dto.Destinatarios.IdsGrupo.FirstOrDefault() == 0)
                    dto.Destinatarios.IdsGrupo.RemoveAt(0);

                filtros.Grupos = dto.Destinatarios.IdsGrupo.Select(x => new PromocionDeGrupos() { IdGrupo = x }).ToList();
            }
        }

        private async Task<Promocion> MappearDtoAPromocion(IPromocionDto dto)
        {
            var promocion = MapperEntidadDto.Mapper(dto, new Promocion());

            await MapearItemsDePromocion(dto, promocion);

            MapearDestinatarios(dto, promocion);
            return promocion;
        }

        private async Task MapearItemsDePromocion(IPromocionDto dto, Promocion promocion)
        {
            var tipoDeItem = await tipoItemServicePromocionService.Get(promocion.IdTipoItem);

            if (tipoDeItem.Nombre == TiposDeItemsDePromocion.Articulos.ToString())
                promocion.ItemsDePromocion = dto.Items.Select(x => (IItemDePromocion)MapperEntidadDto.Mapper(x, new DetallePromocion())).ToList();

            if (tipoDeItem.Nombre == TiposDeItemsDePromocion.Categorias.ToString())
                promocion.ItemsDePromocion = dto.Items.Select(x => (IItemDePromocion)MapperEntidadDto.Mapper(x, new PromocionDeCategoria())).ToList();

            if (tipoDeItem.Nombre == TiposDeItemsDePromocion.Fabricantes.ToString())
                promocion.ItemsDePromocion = dto.Items.Select(x => (IItemDePromocion)MapperEntidadDto.Mapper(x, new PromocionDeFabricantes())).ToList();
        }

        private void MapearDestinatarios(IPromocionDto dto, Promocion promocion)
        {
            //if (dto.Destinatarios.IdsGrupo != null && dto.Destinatarios.IdsGrupo.Any())
            //    promocion.Categorias.AddRange(dto.Destinatarios.IdsGrupo.Select(x => new CategoriaDeArticulo() { Id = x }).ToList());

            if (dto.Destinatarios.IdsProvincia != null && dto.Destinatarios.IdsProvincia.Any())
                promocion.Provincias.AddRange(dto.Destinatarios.IdsProvincia.Select(x => new PromocionDeProvincia() { IdProvincia = x }).ToList());

            if (dto.Destinatarios.IdsRegion != null && dto.Destinatarios.IdsRegion.Any())
                promocion.Regiones.AddRange(dto.Destinatarios.IdsRegion.Select(x => new PromocionDeRegiones() { IdRegion = x }).ToList());

            if (dto.Destinatarios.IdsCliente != null && dto.Destinatarios.IdsCliente.Any())
                promocion.Clientes.AddRange(dto.Destinatarios.IdsCliente.Select(x => new PromocionDeCliente() { IdCliente = x }).ToList());
        }

        #endregion
    }
}
