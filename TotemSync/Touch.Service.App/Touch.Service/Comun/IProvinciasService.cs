using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Service.Comun
{
    public interface IProvinciasService
    {
        Task<IEnumerable<Provincia>> Get();
        Task<Provincia> Get(long id);
        Task<IEnumerable<Provincia>> Get(string nombre);

        Task<ServiceResult> Insert(Provincia provincia);
        Task<ServiceResult> Update(Provincia provincia);
        Task<ServiceResult> Delete(long id);

        Task<IEnumerable<Localidad>> GetLocalidades(long id);
    }
}
