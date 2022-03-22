using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Touch.Core.Comun;
using static Touch.Core.Invariants.InvariantObjects;

namespace Touch.Service.Comun
{
    public class BaseService
    {
        public ServiceResult ServiceResult { get; set; }

        public virtual ServiceResult GetServiceResult(ServiceMethod method, string entityName, bool result = true)
        {
            if (result)
                return new ServiceResult() { HasErrors = false, Message = method + ": Ok", StatusCode = ServiceMethodsStatusCode.Ok , Method = method.ToString()};

            return new ServiceResult() { 
                HasErrors = true, 
                Message = entityName + "- Hubo un error al realizar la acción.", 
                StatusCode = ServiceMethodsStatusCode.Error, 
                Method = method.ToString()
            };
        }

        public ServiceResult GetServiceNonExistantResult(ServiceMethod method, ServiceMethodsStatusCode status, string objeto)
        {
            var result = new ServiceResult()
            {
                Method = method.ToString(),
                StatusCode = status,
                HasErrors = status == ServiceMethodsStatusCode.Error,
                Message = "No existe el " + objeto + " que está buscando"
            };

            return result;
        }

        public ServiceResult GetServiceExistantResult(ServiceMethod method, ServiceMethodsStatusCode status, string objeto)
        {
            var result = new ServiceResult()
            {
                Method = method.ToString(),
                StatusCode = status,
                HasErrors = status == ServiceMethodsStatusCode.Error,
                Message = "Ya existe el/la " + objeto + " que está buscando",
                Notes = "exist"
            };

            return result;
        }

        public ServiceResult ObtenerResultadoDeError(Exception ex, ServiceMethod method)
        {
            return new ServiceResult()
            {
                Method = method.ToString(),
                StatusCode = ServiceMethodsStatusCode.Error,
                HasErrors = true,
                Message = ex.Message
            };
        }
    }
}
