using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Promociones;

namespace Touch.Repositories.Promociones
{
    public class PromocionDeGruposRepository : DestinatarioDePromocionRepository<PromocionDeGrupos>, IPromocionDeGruposRepository
    {
        public PromocionDeGruposRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
