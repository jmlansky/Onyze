using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Touch.Repositories.Comun
{
    public interface ISincronizable<T>
    {
        Task<IEnumerable<T>> GetWithDeleted(DateTime fechaSincro, string[] columnsToIgnore = null);
    }
}
