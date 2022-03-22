using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;
using Touch.Service.Comun;

namespace Touch.Service.Gondolas
{
    public interface IGondolasService : ISingleEntityComunService<Gondola>
    {
        Task<ServiceResult> InsertGondolaConEstantes(Gondola gondola);
        Task<ServiceResult> UpdateGondolaConEstantes(Gondola gondola);

        Task<PagedResult> Get(int? pageNumber, int? pageSize);
        Task<long> GetCurentCount();
        Task<PagedResult> GetWithDeleted(int? pageNumber, int? pageSize, string fechaSincro);

    }
}
