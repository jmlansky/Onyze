using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Repositories.Articulos.Contracts;

namespace Touch.Service.Articulos
{
    public class BusquedaDeArticulosService : IBusquedaDeArticulosService
    {
        private readonly IArticulosRepository articulosRepository;
        public BusquedaDeArticulosService(IArticulosRepository articulosRepository)
        {
            this.articulosRepository = articulosRepository;
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorFabricante(Articulo articulo)
        {
            var list = new List<Articulo>();
            if (!string.IsNullOrWhiteSpace(articulo.Fabricante.Nombre))
                list.AddRange(await articulosRepository.GetArticulosPorFabricante(articulo.Fabricante.Nombre));

            if (articulo.Fabricante.Id > 0)
                list.AddRange(await articulosRepository.GetArticulosPorIdFabricante(articulo.Fabricante.Id));

            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorCategoria(Articulo articulo)
        {
            var list = new List<Articulo>();
            if (!articulo.Categorias.Any())
                return list;

            foreach (var categoria in articulo.Categorias)
            {
                if (!string.IsNullOrWhiteSpace(categoria.Nombre))
                    list.AddRange(await articulosRepository.GetArticulosPorCategoria(categoria.Nombre));

                if (categoria.Id > 0)
                    list.AddRange(await articulosRepository.GetArticulosPorIdCategoria(categoria.Id));
            }

            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorNombre(Articulo articulo)
        {
            var list = new List<Articulo>();
            if (!string.IsNullOrWhiteSpace(articulo.Nombre))
                list.AddRange((IEnumerable<Articulo>)await articulosRepository.Get(articulo.Nombre));
            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorSKU(Articulo articulo)
        {
            var list = new List<Articulo>();
            if (!string.IsNullOrWhiteSpace(articulo.SKU))
                list.AddRange((IEnumerable<Articulo>)await articulosRepository.GetPorSKU(articulo.SKU));
            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorTipo(Articulo articulo)
        {
            var list = new List<Articulo>();
            if (!string.IsNullOrWhiteSpace(articulo.Tipo.Nombre))
                list.AddRange(await articulosRepository.GetArticulosPorTipo(articulo.Tipo.Nombre));

            if (articulo.Tipo.Id > 0)
                list.AddRange(await articulosRepository.GetArticulosPorIdTipo(articulo.Tipo.Id));

            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorAtributos(Articulo articulo)
        {
            var list = new List<Articulo>();
            var nombres = articulo.Atributos.Where(x => !string.IsNullOrWhiteSpace(x.Nombre)).Select(x => x.Nombre);
            if (nombres.Any())
                list.AddRange(await articulosRepository.GetArticulosPorAtributos(nombres));

            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorAtributos(string nombre)
        {
            return (await articulosRepository.GetArticulosPorAtributos(nombre)).Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorCodigoDeBarras(Articulo articulo)
        {
            var list = new List<Articulo>();
            var codigo = articulo.Codigos.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.EAN));
            if (codigo != null && !string.IsNullOrWhiteSpace(codigo.EAN))
                list.AddRange(await articulosRepository.GetArticulosPorCodigo(codigo.EAN));
            return list.Distinct();
        }

        public async Task<IEnumerable<Articulo>> BuscarArticulosPorEstadoActivo(bool activo)
        {
            var list = new List<Articulo>();
            list.AddRange(await articulosRepository.GetArticulosPorEstadoActivo(activo));
            return list.GroupBy(x => x.Id).Select(z => z.First()).Distinct();
        }
    }
}
