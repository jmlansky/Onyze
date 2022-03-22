using Framework.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;
using Touch.Core.Gondolas;
using Touch.Core.Publicaciones;
using Touch.Repositories.Archivos.Contracts;
using Touch.Repositories.Articulos.Contracts;
using Touch.Repositories.Comun;
using Touch.Repositories.Gondolas.Contracts;
using Touch.Repositories.Publicaciones;
using Touch.Service.Archivos.Comun;
using Touch.Service.Archivos.Contracts;
using Touch.Service.Comun;
using static Touch.Core.Invariants.InvariantObjects;
using static Touch.Service.Archivos.Comun.Invariants;

namespace Touch.Service.Archivos.Services
{
    public class ArchivosService : SingleEntityComunService<Archivo>, IArchivosService
    {

        private readonly IArchivosRepository archivosRepository;
        private readonly ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository;
        private readonly ICategoriasDeArticuloRepository categoriasDeArticuloRepository;
        private readonly IGondolasRepository gondolasRepository;
        private readonly IPublicacionesRepository publicationesRepository;
        private readonly IArticulosRepository articulosRepository;
        private readonly IGetServices getServices;
        private readonly IConfiguration configuration;
        private readonly string destinoAlmacenamiento;

        public ArchivosService(ISingleEntityComunRepository<Archivo> comunRepository,
            ISingleEntityComunRepository<TipoArchivo> tipoArchivosRepository,
            IArchivosRepository archivosRepository,
            ICategoriasDeArticuloRepository categoriasDeArticuloRepository,
            IGondolasRepository gondolasRepository,
            IPublicacionesRepository publicationesRepository,
            IArticulosRepository articulosRepository,
            IConfiguration configuration,
            IGetServices getServices) : base(comunRepository)
        {
            this.getServices = getServices;

            this.archivosRepository = archivosRepository;
            this.tipoArchivosRepository = tipoArchivosRepository;
            this.categoriasDeArticuloRepository = categoriasDeArticuloRepository;
            this.gondolasRepository = gondolasRepository;
            this.publicationesRepository = publicationesRepository;
            this.articulosRepository = articulosRepository;

            this.configuration = configuration;
            destinoAlmacenamiento = configuration.GetSection("STORAGE_ENVIRONMENT").Value;
        }

        public async Task<IEnumerable<Archivo>> GetArchivosDelArticulo(long id)
        {
            return await getServices.GetArchivosDelArticulo(id);
        }

        public override Task<Archivo> Get(long id)
        {
            return getServices.Get(id);
        }

        public override async Task<IEnumerable<Archivo>> Get(string name)
        {
            return await getServices.Get(name);
        }

        public override async Task<PagedResult> Get(int? pageNumber, int? pageSize)
        {
            return await getServices.Get(pageNumber, pageSize);
        }

        public async Task<PagedResult> GetFiltrados(FiltroArchivos filtros, int? pageNumber, int? pageSize)
        {
            return await getServices.GetFiltrados(filtros, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Archivo>> GetPorTipo(long idTipo)
        {
            return await getServices.GetPorTipo(idTipo);
        }

        public async Task<IEnumerable<Archivo>> GetPorSize(string size)
        {
            return await getServices.GetPorSize(size);
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndId(long idArchivo, string size)
        {
            return await getServices.GetPorSizeAndId(idArchivo, size);
        }

        public async Task<IEnumerable<Archivo>> GetPorIdOriginal(long idArchivo)
        {
            return await getServices.GetPorIdOriginal(idArchivo);
        }

        public async Task<IEnumerable<Archivo>> GetPorSizeAndTipo(long idTipo, string size)
        {
            return await getServices.GetPorSizeAndTipo(idTipo, size);
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
                    return await base.OnInsertAndGetId(entity, columnsToIgnore);
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

        private string GetAverageColor(Bitmap image)
        {
            int blockSize = 5; // only visit every 5 pixels
            int width, height;

            int r = 0, g = 0, b = 0;
            decimal count = 0;

            height = image.Height;
            width = image.Width;

            for (int i = 0; i < height; i += blockSize)
            {
                for (int j = 0; j < width; j += blockSize)
                {
                    Color color = image.GetPixel(j, i);

                    if (color.A == 0)
                        continue;

                    r += color.R;
                    g += color.G;
                    b += color.B;
                    count++;
                }
            }


            // ~~ used to floor values
            r = (int)Math.Floor(r / count);
            g = (int)Math.Floor(g / count);
            b = (int)Math.Floor(b / count);


            return "rgba(" + r + "," + g + "," + b + ",1)";
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

                if (!Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))

                    using (
                Bitmap image = (Bitmap)Image.FromStream(archivo.File.OpenReadStream()))
                    {
                        archivo.ColorPromedio = GetAverageColor(image);
                        archivo.Width = image.Width;
                        archivo.Height = image.Height;
                    }
                else
                {
                    archivo.ColorPromedio = "";
                    archivo.Width = 0;
                    archivo.Height = 0;
                }


                var filename = GenerarNombreDelArchivo(archivo);
                result = await GuardarArchivoTamañoOriginal(archivo, filename);

                if (Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))
                    return result;

                archivo.NombreGuardado = filename;
                var sizes = new Dictionary<SizeFile, bool>();
                if (small) sizes.Add(SizeFile.Small, true);
                if (medium) sizes.Add(SizeFile.Medium, true);
                if (large) sizes.Add(SizeFile.Large, true);

                await GenerarTodasLasMiniaturasYAlmacenarlas(archivo, sizes);

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
                var t = Task.Run(() =>
                {
                    var archivo = archivosRepository.Get(entity.Id, new string[] { "Tipo", "File", "Miniaturas" }).Result;
                    if (archivo == null || archivo.Id <= 0)
                        throw new Exception("Archivo no existente");

                    var perteneceACategoria = categoriasDeArticuloRepository.EsArchivoDeAlgunaCategoria(archivo.Id).IdArchivo > 0;
                    var perteneceAPublicacion = publicationesRepository.EsArchivoDeAlgunaPublicacion(archivo.Id).IdArchivo > 0;
                    var perteneceAGondolaFondoOEncabezado = gondolasRepository.GetPorIdFondoIdEncabezado(archivo.Id).IdFondo > 0;

                    if (perteneceACategoria || perteneceAPublicacion || perteneceAGondolaFondoOEncabezado)
                        throw new Exception("Archivo asociado a una publicación, categoría o góndola");

                });
                t.Wait();

                return GetServiceResult(ServiceMethod.Delete, "Archivo", await archivosRepository.DeleteAllFromOriginal(entity.Id));
            }
          
            catch (Exception ex)
            {
                string message;

                return new ServiceResult()
                {
                    HasErrors = true,
                    Message = "Hubo un error: " + ex.Message.ToString(),
                    Method = "Delete",
                    StatusCode = ServiceMethodsStatusCode.Error
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

            var filename = GenerarNombreDelArchivo(archivo);
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

                if (archivo.File == null)
                {
                    archivo.Url = archivoExistente.Url;
                    archivo.ColorPromedio = (archivoExistente.ColorPromedio != null) ? archivoExistente.ColorPromedio : "";
                    archivo.Width = archivoExistente.Width;
                    archivo.Height = archivoExistente.Height;
                }
                else
                {


                    if (Path.GetExtension(archivo.File.FileName.ToLower()) != Path.GetExtension(archivo.NombreGuardado.ToLower()))
                    {
                        archivo.NombreGuardado = GenerarNombreDelArchivo(archivo);
                    }

                    if (!Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))

                        using (
                    Bitmap image = (Bitmap)Image.FromStream(archivo.File.OpenReadStream()))
                        {
                            archivo.ColorPromedio = GetAverageColor(image);
                            archivo.Width = image.Width;
                            archivo.Height = image.Height;

                        }
                    else
                    {
                        archivo.ColorPromedio = "";
                        archivo.Width = 0;
                        archivo.Height = 0;
                    }
                }


                result = await ActualizarArchivoOriginal(archivo);

                if (archivo.File != null && Path.GetExtension(archivo.File.FileName.ToLower()).Equals(".mp4"))
                {
                    await QuitarMiniaturas(archivo);
                    return result;

                }


                if (!result.HasErrors)
                {
                    archivo.Miniaturas = (await archivosRepository.GetPorIdOriginal(archivo.Id)).ToList();

                    if (!archivo.Miniaturas.Any())
                    {
                        var sizes = new Dictionary<SizeFile, bool>();

                        sizes.Add(SizeFile.Small, true);
                        sizes.Add(SizeFile.Medium, true);
                        sizes.Add(SizeFile.Large, true);
                        archivo.IdArchivoOriginal = archivo.Id;

                        await GenerarTodasLasMiniaturasYAlmacenarlas(archivo, sizes);
                    }
                    else

                        await ActualizarLasMiniaturasYSubirlasACloud(archivo);
                }


                return result;

            }
            catch (Exception ex)
            {
                return new ServiceResult() { HasErrors = false, Message = ex.Message.ToString(), Method = "Insert", StatusCode = ServiceMethodsStatusCode.Error };
            }
        }

        private async Task<ServiceResult> QuitarMiniaturas(Archivo archivo)
        {

            return GetServiceResult(ServiceMethod.Delete, "Archivo", await archivosRepository.DeleteAllButOriginal(archivo.Id));



        }


        #region métodos privados        

        private async Task<ServiceResult> GuardarArchivoTamañoOriginal(Archivo archivo, string filename)
        {
            archivo.Size = "Original";
            var insertResult = await GuardarArchivoOriginal(archivo, filename);
            if (insertResult.HasErrors)
                throw new Exception("Hubo un error al guardar el archivo en la base");
            return insertResult;
        }

        private async Task<ServiceResult> GuardarArchivoOriginal(Archivo archivo, string filename)
        {
            if (await UploadFile(archivo, filename))
                return await InsertarArchivoEnBaseDeDatos(archivo, filename);

            return GetServiceResult(ServiceMethod.Insert, "Guardar Archivo Original", false);
        }

        private async Task<ServiceResult> InsertarArchivoEnBaseDeDatos(Archivo archivo, string filename)
        {
            archivo.Creado = DateTime.Now;
            archivo.NombreGuardado = filename;

            var columnsToIgnore = new string[] { "Tipo", "File", "Miniaturas" };
            if (archivo.IdArticulo == 0)
                columnsToIgnore = columnsToIgnore.Append("IdArticulo").ToArray();

            var insertResult = await InsertAndGetId(archivo, columnsToIgnore);

            if (insertResult.HasErrors)
            {
                await RemoveFile(new Archivo() { NombreGuardado = filename });
                throw new Exception("Hubo un error al insertar el archivo en la base de datos");
            }

            insertResult.Notes = archivo.Url;

            archivo.IdArchivoOriginal = insertResult.IdObjeto;

            return insertResult;
        }

        private async Task<bool> GenerarMiniaturaYSubirlaACloud(Archivo archivo, string filename, int maxHeight, int maxWidth)
        {
            archivo.NombreGuardado = filename;
            MemoryStream ms = GenerarMiniatura(archivo, maxHeight, maxWidth);
            return await UploadMiniatura(archivo, ms);
        }

        private MemoryStream GenerarMiniatura(Archivo archivo, int maxHeight, int maxWidth)
        {
            var image = ScaleImage(archivo, maxWidth, maxHeight);
            var ms = new MemoryStream();

            ImageFormat formato;
            switch (archivo.File.ContentType.ToLower())
            {
                case "image/png":
                    formato = ImageFormat.Png;
                    break;
                case "image/gif":
                    formato = ImageFormat.Gif;
                    break;
                default:
                    formato = ImageFormat.Jpeg;
                    break;
            }


            image.Save(ms, formato);

            ms.Position = 0;
            return ms;
        }

        private Image ScaleImage(Archivo archivo, int maxWidth, int maxHeight)
        {
            using (var image = Image.FromStream(archivo.File.OpenReadStream()))
            {
                var ratioX = (decimal)maxWidth / image.Width;
                var ratioY = (decimal)maxHeight / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var newWidth = (int)(image.Width * ratio);
                var newHeight = (int)(image.Height * ratio);

                if (image.Width <= newWidth && image.Height <= newHeight)
                {
                    archivo.Width = image.Width;
                    archivo.Height = image.Height;
                    return (Image)image.Clone();
                }

                var newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);

                //Dibuja la imagen, de lo contrario, queda 'imagen negra'
                using (var graphics = Graphics.FromImage(newImage))
                    graphics.DrawImage(image, 0, 0, newWidth, newHeight);


                archivo.Width = newWidth;
                archivo.Height = newHeight;

                newImage.SetResolution(76, 76);
                return newImage;
            }
        }

        private async Task ActualizarLasMiniaturasYSubirlasACloud(Archivo archivo)
        {
            foreach (var miniatura in archivo.Miniaturas)
            {
                var maxWidth = 0;
                var maxHeight = 0;

                miniatura.ColorPromedio = archivo.ColorPromedio;
                miniatura.File = archivo.File;
                miniatura.Nombre = !string.IsNullOrWhiteSpace(archivo.Nombre) ? archivo.Nombre : miniatura.Nombre;

                if (archivo.File != null)
                {

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
                }

                if (archivo.File == null || await GenerarMiniaturaYSubirlaACloud(miniatura, miniatura.NombreGuardado, maxHeight, maxWidth))
                    await ActualizarArchivoEnBaseDeDatos(miniatura);
            }

        }

        private string GenerarNombreDelArchivo(Archivo archivo)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            var filename = timestamp + archivo.File.FileName;
            return filename;
        }

        private async Task GenerarTodasLasMiniaturasYAlmacenarlas(Archivo archivo, Dictionary<SizeFile, bool> sizes)
        {
            var idArchivoOriginal = archivo.IdArchivoOriginal;
            foreach (var item in sizes)
            {
                archivo.Size = item.Key.ToString();
                archivo.IdArchivoOriginal = idArchivoOriginal;
                var parameters = ImageSizes.ResourceManager.GetString(archivo.Size).Split(",").Select(x => Convert.ToInt32(x)).ToArray();
                if (await GenerarMiniaturaYSubirlaACloud(archivo, archivo.NombreGuardado, parameters[0], parameters[1]))
                    await InsertarArchivoEnBaseDeDatos(archivo, archivo.NombreGuardado);
            }
        }

        private async Task<ServiceResult> ActualizarArchivoOriginal(Archivo archivo)
        {
            archivo.Size = "Original";

            var result = GetServiceResult(ServiceMethod.Update, "Archivo Original", false);
            if (archivo.File == null || await UploadFile(archivo, archivo.NombreGuardado))
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
                await RemoveFile(archivo);
                throw new Exception("Hubo un error al insertar el archivo en la base de datos");
            }

            var insertResult = GetServiceResult(ServiceMethod.Update, "Archivo", true);
            insertResult.Notes = archivo.Url;

            return insertResult;
        }

        private async Task<bool> UploadFile(Archivo archivo, string filename)
        {
            //var almacenamiento = InstancesHelper.GetInstanciaAplicar<IAlmacenamientoDeArchivos>(destinoAlmacenamiento);
            var almacenamiento = InstancesHelper.GetImplementation<IAlmacenamientoDeArchivos>(destinoAlmacenamiento, configuration);
            return await almacenamiento.GuardarArchivo(archivo, filename);
        }

        private async Task<bool> UploadMiniatura(Archivo archivo, MemoryStream ms)
        {
            var almacenamiento = InstancesHelper.GetImplementation<IAlmacenamientoDeArchivos>(destinoAlmacenamiento, configuration);
            return await almacenamiento.GuardarMiniaturas(archivo, ms);
        }

        private async Task<Azure.Response<bool>> RemoveFile(Archivo archivo)
        {
            var almacenamiento = InstancesHelper.GetImplementation<IAlmacenamientoDeArchivos>(destinoAlmacenamiento, configuration);
            return await almacenamiento.RemoveFile(archivo);
        }

        #endregion
    }
}
