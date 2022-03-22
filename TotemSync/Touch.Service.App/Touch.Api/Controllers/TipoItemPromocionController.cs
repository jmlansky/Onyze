using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Promociones;
using Touch.Core.Promociones;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
     public class TipoItemPromocionController : BaseController
    {
        private readonly ISingleEntityComunService<TipoItemPromocion> repository;
        public TipoItemPromocionController(IConfiguration configuration, ISingleEntityComunService<TipoItemPromocion> repository): base(configuration)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Get()
        {
            var result = await repository.Get();
            var dto = new List<TipoItemDto>();
            if (result.Any())
                dto = result.Select(x => MapperEntidadDto.Mapper(x, new TipoItemDto())).ToList();

            return Ok(dto);
        }
    }
}
