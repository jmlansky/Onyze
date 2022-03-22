using Azure;
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
    [DestinoDeAlmacenamientoDeArchivos("Local")]
    public class AlmacenamientoDeArchivosLocal : IAlmacenamientoDeArchivos
    {
        private readonly string connectionString = string.Empty;
        private readonly string mainFolder = string.Empty;

        public AlmacenamientoDeArchivosLocal(IConfiguration configuration)
        {
            connectionString = configuration.GetSection("STORAGE_CONNECTION_STRING").Value;
            mainFolder = configuration.GetSection("STORAGE_MAIN_FOLDER").Value;
        }

        public Task<bool> GuardarArchivo(Archivo archivo, string filename)
        {
            try
            {
                var file = archivo.File.OpenReadStream();
                var path = Path.Combine(connectionString, mainFolder, archivo.Size);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                FileStream fileStream = File.Create(Path.Combine(connectionString, mainFolder, archivo.Size, filename), (int)file.Length);
                GuardarArchivoEnElDisco(file, fileStream);

                archivo.Url = '/' + Path.Combine(mainFolder, archivo.Size, filename).Replace('\\', '/');
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> GuardarMiniaturas(Archivo archivo, MemoryStream ms)
        {
            var path = Path.Combine(connectionString, mainFolder, archivo.Size);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(connectionString, mainFolder, archivo.Size, archivo.NombreGuardado);
            using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = new byte[ms.Length];
                ms.Read(bytes, 0, (int)ms.Length);
                file.Write(bytes, 0, bytes.Length);
                ms.Close();
            }

            archivo.Url = '/' + Path.Combine(mainFolder, archivo.Size, archivo.NombreGuardado).Replace('\\', '/');
            return Task.FromResult(true);
        }

        public Task<Response<bool>> RemoveFile(Archivo archivo)
        {
            throw new NotImplementedException();
        }

        private void GuardarArchivoEnElDisco(Stream file, FileStream fileStream)
        {
            // Initialize the bytes array with the stream length and then fill it with data
            byte[] bytesInStream = new byte[file.Length];
            file.Read(bytesInStream, 0, bytesInStream.Length);

            // Use write method to write to the file specified above
            fileStream.Write(bytesInStream, 0, bytesInStream.Length);

            //Close the filestream
            fileStream.Close();
        }
    }
}
