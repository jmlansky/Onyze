using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface IArticuloMultipleRepository: IRepository
    {
        void SetNombreTabla(string nombreTabla);
        Task<bool> DeletAllFromArticulo(long id);

        Task<IEnumerable<Relacion>> GetRelaciones(DateTime fechaSincro, string[] columnsToIgnore = null);
    }
}
