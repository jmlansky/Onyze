using Framework.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Api.Dtos.Test;
using Touch.Core.Test;
using Touch.Service.Comun;

namespace Touch.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ControllerBase
    {
        private readonly ISingleEntityComunService<TestClass> singleEntityService;
        public TestController(ISingleEntityComunService<TestClass> singleEntityService)
        {
            this.singleEntityService = singleEntityService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var result = await singleEntityService.Get();
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        [Authorize(Roles = "Admin, Super, Guest")]
        public async Task<IActionResult> Post([FromBody] TestPostDto testPostDto)
        {
            //MapperEntidadDto --> esta en el Framework
            //1er parametro de la funcion Mapper = objeto a mapear
            //2do parametro, objeto de salida.
            var test = MapperEntidadDto.Mapper(testPostDto, new TestClass());
            var result = await singleEntityService.Insert(test);
            return Ok(result);
        }
    }
}
