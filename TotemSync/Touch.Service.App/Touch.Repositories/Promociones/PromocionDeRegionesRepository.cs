using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Promociones;

namespace Touch.Repositories.Promociones
{
    public class PromocionDeRegionesRepository: DestinatarioDePromocionRepository<PromocionDeRegiones>, IPromocionDeRegionesRepository
    {
        public PromocionDeRegionesRepository(IConfiguration configuration): base (configuration)
        {

        }
    }
}
