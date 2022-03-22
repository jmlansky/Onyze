using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Repositories.Articulos.Contracts;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Articulos
{
    public class CategoriasDeArticulosService : BaseService, ICategoriasDeArticulosService
    {
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;
        private readonly IArchivosService archivosService;
        public CategoriasDeArticulosService(ICategoriasDeArticuloRepository categoriasDeArticuloRepository,
            IArchivosService archivosService)
        {
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
            this.archivosService = archivosService;
        }

        public async Task<ServiceResult> Delete(long id)
        {
            var result = GetServiceResult(ServiceMethod.Delete, "Categoria de artículo", await categoriasDeArticuloRepository.Delete(new CategoriaDeArticulo() { Id = id }));
            if (!result.HasErrors)
            {
                var categorias = ((List<CategoriaDeArticulo>)await categoriasDeArticuloRepository.Get()).Where(x => x.IdCategoriaPadre == id).ToList();

                foreach (var categoria in categorias)
                {
                    var subResult = GetServiceResult(ServiceMethod.Delete, "Categoria de artículo", await categoriasDeArticuloRepository.Delete(new CategoriaDeArticulo() { Id = categoria.Id }));
                    if (subResult.HasErrors)
                    {
                        result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                        result.Message = subResult.Message;
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<CategoriaDeArticulo>> Get(string nombre)
        {
            var categorias = (List<CategoriaDeArticulo>)await categoriasDeArticuloRepository.Get(nombre);
            foreach (var categoria in categorias)
            {
                if (categoria.IdArchivo.HasValue)
                    categoria.Archivo = await archivosService.Get((long)categoria.IdArchivo);

                categoria.Subcategorias = categorias.Where(x => x.IdCategoriaPadre == categoria.Id).ToList();
            }
            return categorias;
        }

        public async Task<IEnumerable<CategoriaDeArticulo>> Get()
        {
            var categorias = (List<CategoriaDeArticulo>)await categoriasDeArticuloRepository.Get();
            foreach (var categoria in categorias)
            {
                if (categoria.IdArchivo.HasValue)
                    categoria.Archivo = await archivosService.Get((long)categoria.IdArchivo);

                categoria.Subcategorias = categorias.Where(x => x.IdCategoriaPadre == categoria.Id).ToList();
            }

            // categorias.RemoveAll(x => x.IdCategoriaPadre > 0);

            return categorias;
        }

        public async Task<CategoriaDeArticulo> Get(long id)
        {
            var categorias = (List<CategoriaDeArticulo>)await categoriasDeArticuloRepository.Get();

            foreach (var cat in categorias)
            {
                if (cat.IdArchivo.HasValue)
                    cat.Archivo = await archivosService.Get((long)cat.IdArchivo);

                cat.Subcategorias = categorias.Where(x => x.IdCategoriaPadre == cat.Id).ToList();
            }

            var categoria = categorias.FirstOrDefault(x => x.Id == id);
            if (categoria.IdCategoriaPadre > 0)
                categoria.NombreCategoriaPadre = categorias.FirstOrDefault(x => x.Id == categoria.IdCategoriaPadre).Nombre;

            return categoria;
        }

        public async Task<ServiceResult> Insert(CategoriaDeArticulo categoria)
        {
            var result = await ValidarCategoriaParaInsertar(categoria);
            if (result.HasErrors)
                return result;

            string[] columnsToIgnore = GetColumnsToIgnoreForInsert(categoria);            

            categoria.Creado = DateTime.Now;
            var resultadoDeInsertCategoria = await categoriasDeArticuloRepository.InsertAndGetId(categoria, columnsToIgnore);

            if (!result.HasErrors)
            {
                result.IdObjeto = resultadoDeInsertCategoria;
                result.StatusCode = ServiceMethodsStatusCode.Ok;
                result.Message = "Categoria - Se ha insertado correctamente";
                result.Method = "Insert";

                await InsertarSubcategorias(categoria, result);
            }

            return result;
        }

        public async Task<ServiceResult> Update(CategoriaDeArticulo categoria)
        {
            var categorias = await categoriasDeArticuloRepository.Get();

            if (categorias.Any(x => x.Id != categoria.Id &&  x.Nombre.ToUpper().Equals(categoria.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría del artículo");

            if (categoria.IdCategoriaPadre != null && categoria.IdCategoriaPadre > 0)
            {
                var categoriaPadre = categorias.FirstOrDefault(x=> x.Id == categoria.IdCategoriaPadre.Value);
                if (categoriaPadre == null || categoriaPadre.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría padre");
            }

            if (categoria.IdArchivo.HasValue && categoria.IdArchivo > 0)
            {
                var archivo = await archivosService.Get(categoria.IdArchivo.Value);
                if (archivo == null || archivo.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Archivo de la categoría");
            }

            var cate = categorias.FirstOrDefault(x => x.Id == categoria.Id);
            if (cate != null && cate.Id > 0)
                return GetServiceResult(ServiceMethod.Update, "Categoría de artículo", await categoriasDeArticuloRepository.Update(categoria));
            var result = GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría del artículo");

            foreach (var subcategoria in categoria.Subcategorias)
            {
                var subResult = GetServiceResult(ServiceMethod.Update, "Categoría de artículo", await categoriasDeArticuloRepository.Update(subcategoria));
                if (subResult.HasErrors)
                {
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    result.Message = subResult.Message;
                }
            }
            return result;
        }

        public async Task<ServiceResult> AsociarArchivoCategoria(CategoriaDeArticulo categoria)
        {
            var result = await ValidarCategoriasParaActualizar(categoria);
            if (result.HasErrors)
                return result;

            return GetServiceResult(ServiceMethod.Update, "Categoría de artículo", await categoriasDeArticuloRepository.AsociarArchivoCategoria(categoria));
        }

        private async Task<ServiceResult> ValidarCategoriasParaActualizar(CategoriaDeArticulo categoria)
        {
            var categorias = await categoriasDeArticuloRepository.Get(categoria.Nombre);
            if (!categorias.Any(x => x.Nombre.ToUpper().Equals(categoria.Nombre.ToUpper())))
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría del artículo");

            if (categoria.IdCategoriaPadre != null && categoria.IdCategoriaPadre > 0)
            {
                var categoriaPadre = await categoriasDeArticuloRepository.Get(categoria.IdCategoriaPadre.Value);
                if (categoriaPadre == null || categoriaPadre.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría padre");
            }

            if (categoria.IdArchivo.HasValue && categoria.IdArchivo > 0)
            {
                var archivo = await archivosService.Get(categoria.IdArchivo.Value);
                if (archivo == null || archivo.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Archivo de la categoría");
            }

            var cate = await categoriasDeArticuloRepository.Get(categoria.Id);
            if (cate == null || cate.Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría");

            return new ServiceResult() { HasErrors = false, StatusCode = ServiceMethodsStatusCode.Ok };
        }

        private async Task<ServiceResult> ValidarCategoriaParaInsertar(CategoriaDeArticulo categoria)
        {
            var categorias = await categoriasDeArticuloRepository.Get(categoria.Nombre);
            if (categorias.Any(x => x.Nombre.ToUpper().Equals(categoria.Nombre.ToUpper())))
                return GetServiceExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Categoría de artículo");

            if (categoria.IdCategoriaPadre != null && categoria.IdCategoriaPadre > 0)
            {
                var categoriaPadre = await categoriasDeArticuloRepository.Get(categoria.IdCategoriaPadre.Value);
                if (categoriaPadre == null || categoriaPadre.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Categoría padre");
            }

            if (categoria.IdArchivo.HasValue && categoria.IdArchivo > 0)
            {
                var archivo = await archivosService.Get(categoria.IdArchivo.Value);
                if (archivo == null || archivo.Id == 0)
                    return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Archivo de la categoría");
            }

            return new ServiceResult() { HasErrors = false };
        }

        private async Task InsertarSubcategorias(CategoriaDeArticulo categoria, ServiceResult result)
        {
            foreach (var subcategoria in categoria.Subcategorias)
            {
                var subResult = GetServiceResult(ServiceMethod.Insert, "Subcategoria de artículo", await categoriasDeArticuloRepository.Insert(subcategoria));
                if (subResult.HasErrors)
                {
                    result.StatusCode = ServiceMethodsStatusCode.PartialContent;
                    result.Message = "Subcategorías - " + subResult.Message;
                }
            }
        }

        private static string[] GetColumnsToIgnoreForInsert(CategoriaDeArticulo categoria)
        {
            var columnsToIgnore = new string[] { "Subcategorias", "Archivo", "NombreCategoriaPadre" };

            if (categoria.IdCategoriaPadre == null || categoria.IdCategoriaPadre == 0)
                columnsToIgnore = columnsToIgnore.Append("IdCategoriaPadre").ToArray();

            if (categoria.IdArchivo == null || categoria.IdArchivo == 0)
                columnsToIgnore = columnsToIgnore.Append("IdArchivo").ToArray();
            return columnsToIgnore;
        }
    }
}
