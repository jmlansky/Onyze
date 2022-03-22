using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Auth;
using Touch.Repositories.Comun;
using Touch.Repositories.Usuarios.Contracts;

namespace Touch.Repositories.Usuarios
{
    public class RolesRepository : SingleEntityComunRepository<Rol>, IRolesRepository
    {
        public RolesRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
