using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Touch.Core.Archivos;
using Touch.Core.Articulos;
using Touch.Core.Comun;

namespace Touch.Service.Archivos.Contracts
{
    public interface IAlmacenamientoDeArchivos
    {
        Task<bool> GuardarArchivo(Archivo archivo, string filename);
        Task<bool> GuardarMiniaturas(Archivo archivo, MemoryStream ms);
        Task<Azure.Response<bool>> RemoveFile(Archivo archivo);
    }
}
