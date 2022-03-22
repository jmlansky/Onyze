using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Service.Comun
{
    public interface ILocalidadesService
    {
        Task<IEnumerable<Localidad>> Get();
        Task<IEnumerable<Localidad>> Get(string nombre);
        Task<Localidad> Get(long id);

        Task<ServiceResult> Insert(Localidad localidad);
        Task<ServiceResult> Update(Localidad localidad);
        Task<ServiceResult> Delete(long id);
    }
}
