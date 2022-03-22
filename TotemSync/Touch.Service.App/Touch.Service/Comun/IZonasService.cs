using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Service.Comun
{
    public interface IZonasService
    {
        Task<IEnumerable<Zona>> Get();
        Task<Zona> Get(long id);
        Task<IEnumerable<Zona>> Get(string nombre);

        Task<ServiceResult> Insert(Zona zona);
        Task<ServiceResult> Update(Zona zona);
        Task<ServiceResult> Delete(long id);
    }
}
