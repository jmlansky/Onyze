using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Comun;

namespace Touch.Repositories.Comun
{
    public interface IRepository
    {
        Task<ComunEntity> Get(long id);
        Task<IEnumerable<ComunEntity>> Get();
        Task<IEnumerable<ComunEntity>> Get(string nombre);
        Task<IEnumerable<ComunEntity>> GetAll(long id);

        Task<bool> Insert(ComunEntity entity);
        Task<bool> Update(ComunEntity entity);
        Task<bool> Delete(ComunEntity entity);
    }
}
