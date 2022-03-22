using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Totems;

namespace Totems.Sync.Repositories.Contracts
{
    public interface IFranjasHorariasRepository : ISingleEntityComunRepository<FranjaHoraria>
    {
        Task<IEnumerable<FranjaHoraria>> GetPorPeriodo(long id);
    }
}
