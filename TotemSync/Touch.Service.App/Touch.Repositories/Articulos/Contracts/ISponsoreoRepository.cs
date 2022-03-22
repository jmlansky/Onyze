using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface ISponsoreoRepository
    {
        Task<IEnumerable<Sponsoreo>> Get(DateTime fechaVigente);
        Task<IEnumerable<Sponsoreo>> Get(long idArticulo);
    }
}
