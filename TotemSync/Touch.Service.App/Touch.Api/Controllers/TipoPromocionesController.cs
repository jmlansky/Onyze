using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Touch.Api.Dtos.TiposPromociones;
using Touch.Core.Promociones;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
    public class TipoPromocionesController : BaseController
    {
        private readonly ISingleEntityComunService<TipoPromocion> service;
        public TipoPromocionesController(IConfiguration configuration, ISingleEntityComunService<TipoPromocion> service): base(configuration)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await service.Get();
            var dto = new List<TipoPromocionDto>();
            if (result != null && result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new TipoPromocionDto())).ToList();

            return Ok(dto);
        }
    }
}
