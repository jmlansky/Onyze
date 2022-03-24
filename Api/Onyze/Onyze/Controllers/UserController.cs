using Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onyze.Dtos;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Onyze.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("/{dni}")]
        public async Task<IActionResult> Get(string dni)
        {
            try
            {
                if (string.IsNullOrEmpty(dni))
                    return BadRequest("Empty dni");

                var user = await userService.GetByDni(dni);
                if (user == null)
                    return NotFound();

                return Ok(MapToDto(user));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("/filtered")]
        public async Task<IActionResult> GetFilteredAsync([FromBody] UserDto userDto)
        {
            var criteria = new Dictionary<string, object>();
            foreach (var property in userDto.GetType().GetProperties().Where(x => !string.IsNullOrWhiteSpace(x.GetValue(userDto).ToString())))
                criteria.Add(property.Name, property.GetValue(userDto));

            var list = await userService.Get(criteria);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDto userDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userDto.Dni))
                    return BadRequest("Empty dni");

                var user = MapToEntity(userDto);
                var result = await userService.Add(user);
                if (!result)
                    throw new Exception("Error while insert new user");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutAsync([FromBody] UserDto userDto)
        {
            try
            {
                if (string.IsNullOrEmpty(userDto.Dni))
                    return BadRequest("Empty dni");

                var user = MapToEntity(userDto);
                var result = await userService.Update(user);
                if (!result)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("/{dni}")]
        public async Task<IActionResult> Delete(string dni)
        {

            try
            {
                if (string.IsNullOrEmpty(dni))
                    return BadRequest("Empty dni");

                var result = await userService.Delete(dni);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto() { Dni = user.Dni, Name = user.Name, LastName = user.LastName, Age = user.Age };
        }

        private User MapToEntity(UserDto userDto)
        {
            return new User() { Dni = userDto.Dni, Name = userDto.Name, LastName = userDto.LastName, Age = userDto.Age };
        }
    }
}
