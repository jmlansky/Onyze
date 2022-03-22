using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Service.Comun;

namespace Touch.Service.Articulos
{
    public interface ISponsoreadosService: ISingleEntityComunService<Sponsoreo>
    {
        Task<IEnumerable<Sponsoreo>> Get(DateTime fechaVigente);
        Task<IEnumerable<Sponsoreo>> GetFromArticulo(long idArticulo);
        Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaVigente);
        Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaDesde, DateTime fechaHasta);
        Task<IEnumerable<Sponsoreo>> Get(long idArticulo, DateTime fechaDesde, DateTime fechaHasta, DateTime horaDesde, DateTime horaHasta);
    }
}
