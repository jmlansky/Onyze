using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Auth;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Usuarios.Contracts
{
    public interface IRolesRepository : ISingleEntityComunRepository<Rol>
    {
    }
}
