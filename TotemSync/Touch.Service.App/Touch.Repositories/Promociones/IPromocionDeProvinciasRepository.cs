using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Promociones;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Promociones
{
    public interface IPromocionDeProvinciasRepository: ISingleEntityComunRepository<PromocionDeProvincia> , IDestinatarioDePromocionRepository<PromocionDeProvincia>
    {
    }
}
