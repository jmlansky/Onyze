using System.Threading.Tasks;
using Touch.Entities.Core;

namespace Touch.Repositories.Comun
{
    public interface ILogRepository
    {
        Task<bool> Insertar(Log log);
    }
}