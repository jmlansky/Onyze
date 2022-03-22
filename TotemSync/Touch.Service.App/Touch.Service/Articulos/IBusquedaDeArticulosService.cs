using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;

namespace Touch.Service.Articulos
{
    public interface IBusquedaDeArticulosService
    {
        Task<IEnumerable<Articulo>> BuscarArticulosPorFabricante(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorCategoria(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorNombre(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorSKU(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorTipo(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorAtributos(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorAtributos(string nombre);
        Task<IEnumerable<Articulo>> BuscarArticulosPorCodigoDeBarras(Articulo articulo);
        Task<IEnumerable<Articulo>> BuscarArticulosPorEstadoActivo(bool activo);
    }
}
