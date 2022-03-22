using Framework.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Touch.Core.Auth;

namespace Touch.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        private readonly IConfiguration configuration;
        public BaseController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //internal string GenerateJSONWebToken(string rol)
        //{
        //    //return TokenHelper.GenerateJSONWebToken(configuration, rol);
        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[] { new Claim("roles", rol) };

        //    var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
        //      configuration["Jwt:Issuer"],
        //      claims,
        //      expires: DateTime.Now.AddSeconds(300),
        //      signingCredentials: credentials);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}
    }
}
