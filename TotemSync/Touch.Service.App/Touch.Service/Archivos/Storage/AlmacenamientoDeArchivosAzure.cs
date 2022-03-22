using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Service.Archivos.Comun;
using Touch.Service.Archivos.Contracts;

namespace Touch.Service.Archivos.Storage
{
    [DestinoDeAlmacenamientoDeArchivos("Azure")]
    public class AlmacenamientoDeArchivosAzure : IAlmacenamientoDeArchivos
    {
        private readonly string connectionString = string.Empty;
        private readonly string mainFolder = string.Empty;
        private readonly BlobContainerClient containerClient;        

        public AlmacenamientoDeArchivosAzure(IConfiguration configuration)
        {            
            connectionString = configuration.GetSection("STORAGE_CONNECTION_STRING").Value;
            mainFolder = configuration.GetSection("STORAGE_MAIN_FOLDER").Value;

            containerClient = new BlobContainerClient(connectionString, mainFolder);            
        }

        public async Task<bool> GuardarArchivo(Archivo archivo, string filename)
        {
            var blobClient = containerClient.GetBlobClient(filename);
            archivo.Url = await UploadFileToCloud(blobClient, archivo.File.OpenReadStream());
            archivo.Nombre = !string.IsNullOrEmpty(archivo.Nombre) ? archivo.Nombre : archivo.File.FileName;

            return !string.IsNullOrEmpty(archivo.Url);
        }            

        public async Task<bool> GuardarMiniaturas(Archivo archivo, MemoryStream ms)
        {
            var blobClientMiniatura = containerClient.GetBlobClient(archivo.Size + "\\" + archivo.NombreGuardado);

            archivo.Url = await UploadFileToCloud(blobClientMiniatura, ms);
            return !string.IsNullOrEmpty(archivo.Url);
        }

        public async Task<Azure.Response<bool>> RemoveFile(Archivo archivo)
        {
            var containerClient = new BlobContainerClient(connectionString, mainFolder);
            var blobClient = containerClient.GetBlobClient(archivo.NombreGuardado);
            var response = await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return response;
        }

        private async Task<string> UploadFileToCloud(BlobClient blobClient, Stream file)
        {
            using var fs = file;
            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            var response = await blobClient.UploadAsync(fs);
            if (response.GetRawResponse().Status != 201)
                throw new Exception("Error al guardar el archivo en la nube: " + response.GetRawResponse().ReasonPhrase);
            return blobClient.Uri.AbsoluteUri;
        }
    }
}
