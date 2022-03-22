using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Totems;

namespace Totem.Sync.Repositories.Contracts
{
    public interface IProgramacionesRepository : ISingleEntityComunRepository<Programacion>
    {
        Task<List<Programacion>> GetPorIdPlaylist(long id);
    }
}
