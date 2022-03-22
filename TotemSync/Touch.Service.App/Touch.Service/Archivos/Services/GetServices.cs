using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Core.Invariants;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Comun;
using Touch.Service.Archivos.Contracts;

namespace Touch.Service.Archivos.Services
{
    public class GetServices : IGetServices
    {
        private readonly IArchivosRepository archivosRepository;
        private readonly ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository;

        public GetServices(IArchivosRepository archivosRepository,
            ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository)
        {
            this.archivosRepository = archivosRepository;
            this.tipoArchivosRepository = tipoArchivosRepository;
        }

        public async Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            var archivos = await archivosRepository.Get(new string[] { "File", "Tipo", "Miniaturas" });
            var tiposDeArchivo = await tipoArchivosRepository.Get();

            foreach (var archivo in archivos)
                archivo.Tipo = tiposDeArchivo.FirstOrDefault(x => x.Id == archivo.IdTipo);

            var archivosOriginales = archivos.Where(x => x.IdArchivoOriginal == 0);

            var pagedList = new PagedList<Archivo>(archivosOriginales, pageNumber ?? 1, pageSize ?? 25);
            AnidarArchivosConSusMiniaturas(pagedList.ToList(), archivos.OrderBy(x => x.Id));

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, archivosOriginales.Count())
            {
                PagedList = pagedList
            };

            return pagedResult;
        }

        public async Task<PagedResult> GetFiltrados(FiltroArchivos filtros, int? pageNumber, int? pageSize)
        {
            var archivos = await archivosRepository.GetFiltrados(filtros, new string[] { "File", "Tipo", "Miniaturas" });
            var tiposDeArchivo = await tipoArchivosRepository.Get();

            foreach (var archivo in archivos)
                archivo.Tipo = tiposDeArchivo.FirstOrDefault(x => x.Id == archivo.IdTipo);

            var archivosOriginales = archivos.Where(x => x.IdArchivoOriginal == 0);
            var pagedList = new PagedList<Archivo>(archivosOriginales, pageNumber ?? 1, pageSize ?? 25);
            AnidarArchivosConSusMiniaturas(pagedList.ToList(), archivos.OrderBy(x => x.Id));

            var pagedResult = new PagedResult(pageNumber ?? 1, pageSize ?? 25, archivosOriginales.Count())
            {
                PagedList = pagedList
            };

            return pagedResult;
        }

        public async Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id)
        {
            var archivos = await archivosRepository.GetArchivosDelArticulo(id);
            if (InvariantObjects.TiposDeArchivos == null)
                InvariantObjects.TiposDeArchivos = new Dictionary<long, string>();

            if (!InvariantObjects.TiposDeArchivos.Any())
            {
                var tipos = await tipoArchivosRepository.Get();
                foreach (var tipo in tipos)
                    InvariantObjects.TiposDeArchivos.Add(tipo.Id, tipo.Nombre);
            }

            foreach (var archivo in archivos)
            {
                var tipo = InvariantObjects.TiposDeArchivos.FirstOrDefault(x => x.Key.Equals(archivo.IdTipo));
                if (tipo.Key == 0)
                    break;
                archivo.Tipo.Id = tipo.Key;
                archivo.Tipo.Nombre = tipo.Value;
            }

            return archivos;
        }

        public Task<Archivo> Get(long id)
        {
            var archivo = new Archivo();
            var t = Task.Run(() =>
            {
                archivo = archivosRepository.Get(id, new string[] { "Tipo", "Miniaturas", "File" }).Result;
                archivo.Tipo = tipoArchivosRepository.Get(archivo.IdTipo).Result;
                archivo.Miniaturas = archivosRepository.GetPorIdOriginal(id).Result.ToList();
            });
            t.Wait();

            return Task.FromResult(archivo);
        }

        public async Task<IEnumerable<Archivo>> Get(string name)
        {
            var archivos = await archivosRepository.Get(name, new string[] { "File", "Tipo", "Miniaturas" });
            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos.OrderBy(x => x.Id));

            return archivosOriginales;
        }

        public async Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo)
        {
            var archivos = (await archivosRepository.GetPorIdOriginal(idArchivo)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);
            return archivos;
        }

        public async Task<IEnumerable<Archivo>> GetPorSize(string size)
        {
            var archivos = (await archivosRepository.GetPorSize(size)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);

            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos);

            return archivosOriginales;
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size)
        {
            var archivos = (await archivosRepository.GetPorSizeAndId(idArchivo, size)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);

            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos);

            return archivosOriginales;
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size)
        {
            var archivos = (await archivosRepository.GetPorSizeAndTipo(idTipo, size)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);
            return archivos;
        }

        public async Task<IEnumerable<Archivo>> GetPorTipo(long idTipo)
        {
            var archivos = (await archivosRepository.GetPorTipo(idTipo)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);

            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos);

            return archivosOriginales;
        }

        private async Task CompletarTipoDeArchivo(IEnumerable<Archivo> archivos)
        {
            var tipos = await tipoArchivosRepository.Get();
            foreach (var archivo in archivos)
                archivo.Tipo = tipos.FirstOrDefault(x => x.Id == archivo.IdTipo);
        }

        private List<Archivo> AnidarArchivosConSusMiniaturas(IOrderedEnumerable<Archivo> archivos)
        {
            var archivosOriginales = new List<Archivo>();
            archivosOriginales = archivos.Where(x => x.IdArchivoOriginal == 0).ToList();

            foreach (var archivoOriginal in archivosOriginales)
                archivoOriginal.Miniaturas.AddRange(archivos.Where(x => x.IdArchivoOriginal == archivoOriginal.Id));
            return archivosOriginales;
        }

        private void AnidarArchivosConSusMiniaturas(List<Archivo> archivosOriginales, IOrderedEnumerable<Archivo> archivos)
        {
            foreach (var archivoOriginal in archivosOriginales)
                archivoOriginal.Miniaturas.AddRange(archivos.Where(x => x.IdArchivoOriginal == archivoOriginal.Id));
        }        
    }
}
