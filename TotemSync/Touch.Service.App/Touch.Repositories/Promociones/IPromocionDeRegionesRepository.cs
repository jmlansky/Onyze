using System;
using System.Collections.Generic;
using System.Text;
using Touch.Core.Promociones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Promociones
{
    public interface IPromocionDeRegionesRepository : IDestinatarioDePromocionRepository<PromocionDeRegiones>, ISingleEntityComunRepository<PromocionDeRegiones>
    {
    }
}
