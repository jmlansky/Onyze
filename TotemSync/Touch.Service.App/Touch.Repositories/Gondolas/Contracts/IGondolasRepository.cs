using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Gondolas;
using Touch.Repositories.Comun;

namespace Touch.Repositories.Gondolas.Contracts
{
    public interface IGondolasRepository : ISingleEntityComunRepository<Gondola>, ISincronizable<Gondola>
    {
        Task<bool> AsociarArchivoAGondola(Gondola gondola);
        
        /// <summary>
        /// Obtiene la lista de góndolas en las que se encuentra un artículo
        /// </summary>
        /// <param name="id">Id del Artículo</param>
        /// <returns>Lista de Góndolas</returns>
        Task<IEnumerable<Gondola>> GetGondolasDelArticulo(long id);
        Task<long> InsertGondaEstantesArticulos(Gondola gondola, string[] columnsToIgnore = null);
        Task<bool> UpdateGondolaEstantesArticulos(Gondola gondola, string[] columnsToIgnore = null);
        Gondola GetPorIdFondoIdEncabezado(long id);
        Task<long> GetCurentCount();
    }
}
