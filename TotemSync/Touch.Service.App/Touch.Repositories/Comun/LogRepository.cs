using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Touch.Entities.Core;

namespace Touch.Repositories.Comun
{
    public class LogRepository : BaseRepository, ILogRepository
    {
        public LogRepository(IConfiguration configuration) : base(configuration)
        { }

        public Task<bool> Insertar(Log log)
        {
            throw new System.NotImplementedException();
        }

        //public async Task<bool> Insertar(Log log)
        //{
        //    var sql = @"insert into Log(capa, metodo, dato, fecha, idEmpresa, idSucursal, idUsuario, txnExitosa, error) 
        //                values (@capa, @metodo, @dato, @fecha, @idEmpresa, @idSucursal, @idUsuario, @txnExitosa, @error)";
        //    var parameters = new Dictionary<string, object>()
        //    {
        //        {"capa", log.Capa},
        //        {"metodo", log.Metodo},
        //        {"dato", log.Dato},
        //        {"fecha", log.Fecha},
        //        {"idEmpresa", log.IdEmpresa},
        //        {"idSucursal", log.IdSucursal},
        //        {"idUsuario", log.IdUsuario},
        //        {"txnExitosa", log.TxnExitosa},
        //        {"error", log.Error}
        //};
        //    return await ExecuteInsertOrUpdate(sql, parameters);
        //}
    }
}
