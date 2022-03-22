using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Auth;

namespace Framework.Auth.Services.Contracts
{
    public interface IAuthService
    {
        Task<LoginModel> Authenticate(LoginModel login);

        Task<string> GenerateToken(string key, string rolUsuario, long lifeTimeInSeconds);
    }
}
