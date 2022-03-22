using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Promociones
{
    public class PromocionDeProvinciasRepository : DestinatarioDePromocionRepository<PromocionDeProvincia>, IPromocionDeProvinciasRepository
    {
        public PromocionDeProvinciasRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
