using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Service.Comun
{
    public interface IBarriosService
    {
        Task<IEnumerable<Barrio>> Get();
        Task<Barrio> Get(long id);
        Task<IEnumerable<Barrio>> Get(string nombre);
        Task<ServiceResult> Insert(Barrio barrio);
        Task<ServiceResult> Update(Barrio barrio);
        Task<ServiceResult> Delete(long id);
    }
}
