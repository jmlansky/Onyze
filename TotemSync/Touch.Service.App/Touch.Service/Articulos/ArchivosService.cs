using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Invariants;
using Touch.Core.Publicaciones;
using Touch.Repositories.Articulos;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas;
using Touch.Repositories.Publicaciones;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;


namespace Touch.Service.Articulos
{
    public class ArchivosService : SingleEntityComunService<Archivo>, IArchivosService
    {
        private readonly IArchivosRepository archivosRepository;
        private readonly ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;
        private readonly IGondolasRepository gondolasRepository;
        private readonly IPublicacionesRepository publicationesRepository;
        private readonly IArticulosRepository articulosRepository;

        private readonly string connectionString = string.Empty;
        private readonly string container = string.Empty;
        private readonly BlobContainerClient containerClient;

        public ArchivosService(ISingleEntityComunRepository<Archivo> comunRepository,
            ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository,
            IArchivosRepository archivosRepository,
            ICategoriasDeArticuloRepository categoriasDeArticuloRepository,
            IGondolasRepository gondolasRepository,
            IPublicacionesRepository publicationesRepository,
            IArticulosRepository articulosRepository,
            IConfiguration configuration) : base(comunRepository)
        {
            this.archivosRepository = archivosRepository;
            this.tipoArchivosRepository = tipoArchivosRepository;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
            this.gondolasRepository = gondolasRepository;
            this.publicationesRepository = publicationesRepository;
            this.articulosRepository = articulosRepository;

            connectionString = configuration.GetSection("AzureStorage:ConnectionString").Value;
            container = configuration.GetSection("AzureStorage:Container").Value;
            containerClient = new BlobContainerClient(connectionString, container);
        }


        public async Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id)
        {
            var archivos = await archivosRepository.GetArchivosDelArticulo(id);
            if (TiposDeArchivos == null)
                TiposDeArchivos = new Dictionary<long, string>();

            if (!InvariantObjects.TiposDeArchivos.Any())
            {
                var tipos = await tipoArchivosRepository.Get();
                foreach (var tipo in tipos)
                    TiposDeArchivos.Add(tipo.Id, tipo.Nombre);
            }

            foreach (var archivo in archivos)
            {
                var tipo = TiposDeArchivos.FirstOrDefault(x => x.Key.Equals(archivo.IdTipo));
                if (tipo.Key == 0)
                    break;
                archivo.Tipo.Id = tipo.Key;
                archivo.Tipo.Nombre = tipo.Value;
            }

            return archivos;
        }

        public override Task<Archivo> Get(long id)
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

        public override async Task<IEnumerable<Archivo>> Get(string name)
        {
            var archivos = await archivosRepository.Get(name, new string[] { "File", "Tipo", "Miniaturas" });
            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos.OrderBy(x => x.Id));

            return archivosOriginales;
        }

        public override async Task<PagedResult> Get(int? pageNumber, int? pageSize)
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

        public override async Task<ServiceResult> Insert(Archivo entity)
        {
            try
            {
                var archivos = await archivosRepository.Get(entity.Nombre, new string[] { "Tipo", "File", "Miniaturas" });
                if (!archivos.Where(x => entity.Url.ToLower() == x.Url.ToLower()).Any())
                {
                    var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas" };
                    if (entity.IdArticulo == 0)
                        columnsToIgnore = columnsToIgnore.Append("IdArticulo").ToArray();
                    return await base.onInsertAndGetId(entity, columnsToIgnore);
                }

                return GetServiceResult(ServiceMethod.Insert, "Archivo", false);
            }
            catch (Exception ex)
            {
                var result = GetServiceResult(ServiceMethod.Insert, "Arhivo", false);
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ServiceResult> UploadFile(Archivo archivo, bool small, bool medium, bool large)
        {
            try
            {
                var result = new ServiceResult();
                if ((await tipoArchivosRepository.Get(archivo.IdTipo)).Id <= 0)
                    result = GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de Archivo");

                if (result.HasErrors)
                    return result;

                var filename = ObtenerNombreDelArchivo(archivo);
                result = await GuardarArchivoTamañoOriginal(archivo, filename);

                if (Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))
                    return result;

                await GenerarTodasLasMiniaturasYSubirlasACloud(archivo, small, medium, large, filename);

                return result;

            }
            catch (Exception ex)
            {
                return new ServiceResult() { HasErrors = false, Message = ex.Message.ToString(), Method = "Insert", StatusCode = ServiceMethodsStatusCode.Error };
            }
        }

        public override async Task<ServiceResult> Delete(Archivo entity)
        {
            try
            {
                var archivo = await archivosRepository.Get(entity.Id, new string[] { "Tipo", "File", "Miniaturas" });
                if (archivo == null || archivo.Id <= 0)
                    return GetServiceNonExistantResult(InvariantObjects.ServiceMethod.Delete, InvariantObjects.ServiceMethodsStatusCode.Error, "Archivo");

                bool perteneceACategoria = false;
                bool perteneceAPublicacion = false;

                var t = Task.Run(() =>
                {
                    perteneceACategoria = categoriasDeArticuloRepository.EsArchivoDeAlgunaCategoria(archivo.Id).IdArchivo > 0;
                    perteneceAPublicacion = publicationesRepository.EsArchivoDeAlgunaPublicacion(archivo.Id).IdArchivo > 0;
                });
                t.Wait();

                if (perteneceACategoria || perteneceAPublicacion)
                    return GetServiceResult(ServiceMethod.Delete, "Archivo asociado a una publicación o categoría", false);


                await archivosRepository.Delete(entity);
                await archivosRepository.DeleteAllFromOriginal(entity.Id);

                return GetServiceResult(ServiceMethod.Delete, "Archivo", true);
            }
            catch (Exception ex)
            {
                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = "Hubo un error: " + ex.Message.ToString(),
                    Method = "Delete",
                    StatusCode = InvariantObjects.ServiceMethodsStatusCode.Error
                };
            }
        }

        public async Task<ServiceResult> UploadFileAsociarAlArticulo(long idArticulo, Archivo archivo, bool small, bool medium, bool large)
        {

            if ((await articulosRepository.Get(idArticulo)).Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Artículo");

            archivo.IdArticulo = idArticulo;
            return await UploadFile(archivo, small, medium, large);
        }

        public async Task<ServiceResult> UploadFileAsociarAGondola(long idGondola, string referencia, Archivo archivo, bool small)
        {
            if ((await gondolasRepository.Get(idGondola)).Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Góndola");

            if ((await tipoArchivosRepository.Get(archivo.IdTipo)).Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de Archivo");

            if (Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))
                return GetServiceResult(ServiceMethod.Update, "Archivo de gondola - no puede ser video", false);

            var filename = ObtenerNombreDelArchivo(archivo);
            var result = await GuardarArchivoTamañoOriginal(archivo, filename);
            if (result.HasErrors)
                return GetServiceResult(ServiceMethod.Insert, "Archivos de la góndola", false);

            if (!result.HasErrors && small)
            {
                archivo.Size = "Small";
                if (await GenerarMiniaturaYSubirlaACloud(archivo, filename, 384, 216))
                    await InsertarArchivoEnBaseDeDatos(archivo, filename);
            }

            var gondola = new Gondola()
            {
                Id = idGondola,
                IdEncabezado = referencia.ToLower().Equals("encabezado") ? result.IdObjeto : 0,
                IdFondo = referencia.ToLower().Equals("fondo") ? result.IdObjeto : 0
            };

            if (await gondolasRepository.AsociarArchivoAGondola(gondola))
                return result;

            return new ServiceResult()
            {
                HasErrors = true,
                StatusCode = ServiceMethodsStatusCode.Error,
                IdObjeto = result.IdObjeto,
                Message = "Hubo un error al asociar el archivo a la Góndola",
                Method = "UploadFileAsociarAGondola"
            };
        }

        public async Task<ServiceResult> UploadFileAsociarACategoria(long idCategoria, Archivo archivo, bool small, bool medium, bool large)
        {
            if ((await categoriasDeArticuloRepository.Get(idCategoria)).Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Categoría");

            var result = await UploadFile(archivo, small, medium, large);
            if (result.HasErrors)
                return GetServiceResult(ServiceMethod.Insert, "Archivo de la categoría", false);

            if (await categoriasDeArticuloRepository.AsociarArchivoCategoria(new CategoriaDeArticulo() { Id = idCategoria, IdArchivo = result.IdObjeto }))
                return result;

            return new ServiceResult()
            {
                HasErrors = true,
                StatusCode = ServiceMethodsStatusCode.Error,
                IdObjeto = result.IdObjeto,
                Message = "Hubo un error al asociar el archivo a la categoría",
                Method = "UploadFileAsociarACategoria"
            };
        }

        public async Task<ServiceResult> UploadFileAsociarAPublicacion(long idPublicacion, Archivo archivo, bool small, bool medium, bool large)
        {
            if ((await publicationesRepository.Get(idPublicacion)).Id <= 0)
                return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Publicación");

            var result = await UploadFile(archivo, small, medium, large);
            if (result.HasErrors)
                return GetServiceResult(ServiceMethod.Insert, "Archivo de la publicación", false);

            if (await publicationesRepository.AsociarArchivoPublicacion(new Publicacion() { Id = idPublicacion, IdArchivo = result.IdObjeto }))
                return result;

            return new ServiceResult()
            {
                HasErrors = true,
                StatusCode = ServiceMethodsStatusCode.Error,
                IdObjeto = result.IdObjeto,
                Message = "Hubo un error al asociar el archivo a la publicación",
                Method = "UploadFileAsociarAPublicacion"
            };
        }

        public async Task<ServiceResult> UpdateFile(Archivo archivo)
        {
            try
            {
                var archivoExistente = await archivosRepository.Get(archivo.Id, new string[] { "File", "Tipo", "Miniaturas" });
                if (archivoExistente.Id <= 0)
                    return GetServiceNonExistantResult(ServiceMethod.Update, ServiceMethodsStatusCode.Error, "Archivo");

                if (!archivoExistente.Size.ToLower().Equals("original"))
                    return GetServiceResult(ServiceMethod.Update, "El archivo es una miniatura", false);

                var result = new ServiceResult();
                if ((await tipoArchivosRepository.Get(archivo.IdTipo)).Id <= 0)
                    result = GetServiceNonExistantResult(ServiceMethod.Insert, ServiceMethodsStatusCode.Error, "Tipo de Archivo");

                if (result.HasErrors)
                    return result;

                archivo.NombreGuardado = archivoExistente.NombreGuardado;
                result = await ActualizarArchivoOriginal(archivo);

                if (Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))
                    return result;

                if (!result.HasErrors)
                {
                    archivo.Miniaturas = (await archivosRepository.GetPorIdOriginal(archivo.Id)).ToList();
                    await ActualizarLasMiniaturasYSubirlasACloud(archivo);
                }

                return result;

            }
            catch (Exception ex)
            {
                return new ServiceResult() { HasErrors = false, Message = ex.Message.ToString(), Method = "Insert", StatusCode = ServiceMethodsStatusCode.Error };
            }
        }

        public async Task<IEnumerable<Archivo>> GetPorTipo(long idTipo)
        {

            var archivos = (await archivosRepository.GetPorTipo(idTipo)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);

            var archivosOriginales = AnidarArchivosConSusMiniaturas(archivos);

            return archivosOriginales;
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

        public async Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo)
        {
            var archivos = (await archivosRepository.GetPorIdOriginal(idArchivo)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);
            return archivos;
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size)
        {
            var archivos = (await archivosRepository.GetPorSizeAndTipo(idTipo, size)).OrderBy(x => x.Id);
            await CompletarTipoDeArchivo(archivos);
            return archivos;
        }

        #region métodos privados
        private async Task<Azure.Response<bool>> RemoveFromCloud(Archivo archivo)
        {
            var containerClient = new BlobContainerClient(connectionString, container);

            var blobClient = containerClient.GetBlobClient(archivo.NombreGuardado);
            var response = await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
            return response;
        }

        private async Task<ServiceResult> GuardarArchivoOriginal(Archivo archivo, string filename)
        {
            if (await UploadFile(archivo, filename))
                return await InsertarArchivoEnBaseDeDatos(archivo, filename);

            return GetServiceResult(ServiceMethod.Insert, "Guardar Archivo Original", false);
        }

        private async Task<bool> UploadFile(Archivo archivo, string filename)
        {
            var blobClient = containerClient.GetBlobClient(filename);
            archivo.Url = await UploadFileToCloud(blobClient, archivo.File.OpenReadStream());
            archivo.Nombre = !string.IsNullOrEmpty(archivo.Nombre) ? archivo.Nombre : archivo.File.FileName;

            return !string.IsNullOrEmpty(archivo.Url);
        }

        private async Task<string> UploadFileToCloud(BlobClient blobClient, Stream file)
        {
            using (var fs = file)
            {
                await blobClient.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
                var response = await blobClient.UploadAsync(fs);
                if (response.GetRawResponse().Status != 201)
                    throw new Exception("Error al guardar el archivo en la nube: " + response.GetRawResponse().ReasonPhrase);
                return blobClient.Uri.AbsoluteUri;
            }
        }

        private async Task<ServiceResult> GuardarArchivoTamañoOriginal(Archivo archivo, string filename)
        {
            archivo.Size = "Original";
            var insertResult = await GuardarArchivoOriginal(archivo, filename);
            if (insertResult.HasErrors)
                throw new Exception("Hubo un error al guardar el archivo en la base");
            return insertResult;
        }

        private async Task<ServiceResult> InsertarArchivoEnBaseDeDatos(Archivo archivo, string filename)
        {
            archivo.Creado = DateTime.Now;
            archivo.NombreGuardado = filename;

            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas" };
            if (archivo.IdArticulo == 0)
                columnsToIgnore = columnsToIgnore.Append("IdArticulo").ToArray();

            var insertResult = await onInsertAndGetId(archivo, columnsToIgnore);

            if (insertResult.HasErrors)
            {
                await RemoveFromCloud(new Archivo() { NombreGuardado = filename });
                throw new Exception("Hubo un error al insertar el archivo en la base de datos");
            }

            insertResult.Notes = archivo.Url;

            archivo.IdArchivoOriginal = insertResult.IdObjeto;

            return insertResult;
        }

        private Image ScaleImage(IFormFile file, int maxWidth, int maxHeight)
        {
            using (var image = Image.FromStream(file.OpenReadStream()))
            {
                var ratioX = (decimal)maxWidth / image.Width;
                var ratioY = (decimal)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                var newImage = new Bitmap(newWidth, newHeight);

                //Dibuja la imagen, de lo contrario, queda 'imagen negra'
                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);

                newImage.SetResolution(76, 76);
                return newImage;
            }
        }

        private async Task<bool> GenerarMiniaturaYSubirlaACloud(Archivo archivo, string filename, int maxHeight, int maxWidth)
        {
            var image = ScaleImage(archivo.File, maxWidth, maxHeight);
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;

            var blobClientMiniatura = containerClient.GetBlobClient(archivo.Size + "\\" + filename);

            archivo.Url = await UploadFileToCloud(blobClientMiniatura, ms);

            return !string.IsNullOrEmpty(archivo.Url);
        }

        private async Task ActualizarLasMiniaturasYSubirlasACloud(Archivo archivo)
        {
            foreach (var miniatura in archivo.Miniaturas)
            {
                var maxWidth = 0;
                var maxHeight = 0;
                miniatura.File = archivo.File;
                miniatura.Nombre = !string.IsNullOrWhiteSpace(archivo.Nombre) ? archivo.Nombre : miniatura.Nombre;

                if (miniatura.Size.ToLower().Equals("small"))
                {
                    maxWidth = 240;
                    maxHeight = 240;
                }

                if (miniatura.Size.ToLower().Equals("medium"))
                {
                    maxWidth = 480;
                    maxHeight = 480;
                }

                if (miniatura.Size.ToLower().Equals("large"))
                {
                    maxWidth = 1000;
                    maxHeight = 1000;
                }

                if (await GenerarMiniaturaYSubirlaACloud(miniatura, miniatura.NombreGuardado, maxHeight, maxWidth))
                    await ActualizarArchivoEnBaseDeDatos(miniatura);
            }

        }

        private string ObtenerNombreDelArchivo(Archivo archivo)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            var filename = timestamp + archivo.File.FileName;
            return filename;
        }

        private async Task GenerarTodasLasMiniaturasYSubirlasACloud(Archivo archivo, bool small, bool medium, bool large, string filename)
        {
            var idArchivoOriginal = archivo.IdArchivoOriginal;
            if (small)
            {
                archivo.Size = "Small";
                if (await GenerarMiniaturaYSubirlaACloud(archivo, filename, 240, 240))
                    await InsertarArchivoEnBaseDeDatos(archivo, filename);
            }


            if (medium)
            {
                archivo.Size = "Medium";
                archivo.IdArchivoOriginal = idArchivoOriginal;
                if (await GenerarMiniaturaYSubirlaACloud(archivo, filename, 480, 480))
                    await InsertarArchivoEnBaseDeDatos(archivo, filename);
            }

            if (large)
            {
                archivo.Size = "Large";
                archivo.IdArchivoOriginal = idArchivoOriginal;
                if (await GenerarMiniaturaYSubirlaACloud(archivo, filename, 1000, 1000))
                    await InsertarArchivoEnBaseDeDatos(archivo, filename);
            }
        }

        private async Task<ServiceResult> ActualizarArchivoOriginal(Archivo archivo)
        {
            archivo.Size = "Original";

            var result = GetServiceResult(ServiceMethod.Update, "Archivo Original", false);
            if (await UploadFile(archivo, archivo.NombreGuardado))
                result = await ActualizarArchivoEnBaseDeDatos(archivo);

            return result;
        }

        private async Task<ServiceResult> ActualizarArchivoEnBaseDeDatos(Archivo archivo)
        {
            archivo.Modificado = DateTime.Now;

            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas" };
            if (archivo.IdArticulo == 0)
                columnsToIgnore = columnsToIgnore.Append("IdArticulo").ToArray();

            var result = await archivosRepository.Update(archivo, columnsToIgnore);

            if (!result)
            {
                await RemoveFromCloud(archivo);
                throw new Exception("Hubo un error al insertar el archivo en la base de datos");
            }

            var insertResult = GetServiceResult(ServiceMethod.Update, "Archivo", true);
            insertResult.Notes = archivo.Url;

            return insertResult;
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

        private async Task CompletarTipoDeArchivo(IEnumerable<Archivo> archivos)
        {
            var tipos = await tipoArchivosRepository.Get();
            foreach (var archivo in archivos)
                archivo.Tipo = tipos.FirstOrDefault(x => x.Id == archivo.IdTipo);
        }
        #endregion
    }
}
