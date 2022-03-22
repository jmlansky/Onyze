using Framework.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Totems;

namespace Totems.Sync.Repositories.Contracts
{
    public interface IPeriodosRepository : ISingleEntityComunRepository<Periodo>
    {
        Task<IEnumerable<Periodo>> GetPorIdProgramacion(long id);
    }
}
