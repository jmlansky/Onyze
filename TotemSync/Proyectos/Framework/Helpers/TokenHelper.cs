using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Framework.Helpers
{
    public static class TokenHelper
    {       

        public static string GenerateJSONWebToken(IConfiguration configuration, string rol)
        {
            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[] { new Claim("roles", rol) };

                var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                  configuration["Jwt:Issuer"],
                  claims,
                  expires: DateTime.Now.AddSeconds(7200),
                  signingCredentials: credentials);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
    }
}
