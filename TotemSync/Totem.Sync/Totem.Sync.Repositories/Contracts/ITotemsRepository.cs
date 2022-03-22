using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CoreTotem = Touch.Core.Totems;

namespace Totem.Sync.Repositories.Contracts
{
    public interface ITotemsRepository : ISingleEntityComunRepository<CoreTotem.Totem>
    {
        Task<List<CoreTotem.Totem>> Syncronizar(long idTotem, DateTime fecha);
    }
}
