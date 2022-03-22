using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Articulos.Contracts
{
    public interface IFabricantesRepository: IRepository
    {
        Task<Fabricante> GetFabricante(long articuloId);       
    }
}
