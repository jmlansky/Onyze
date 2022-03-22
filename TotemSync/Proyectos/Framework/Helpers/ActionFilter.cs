using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Framework.Auth;

namespace Framework.Helpers
{
    public class ActionFilter : IActionFilter
    {
        private readonly IConfiguration configuration;
        private string encryptedRol { get; set; }

        public ActionFilter(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (string.IsNullOrEmpty(encryptedRol))
            {
                var rolRequest = context.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "rol");

                var actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

                if (actionName != "Login")
                {
                    var allowsAnonymus = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name == "AllowAnonymousAttribute") != null;
                    if (!allowsAnonymus)
                    {

                        if (rolRequest.Key == null)
                            context.Result = new UnauthorizedObjectResult("user is unauthorized");
                        else
                        {
                            encryptedRol = rolRequest.Value;
                            try
                            {
                                var decription = Encryption.Decrypt(encryptedRol).Split(";");
                                if (decription.Length != 2 || !ActiveUsers.SesionesActivas.Any(x => x.Key.Equals(encryptedRol)))
                                    context.Result = new UnauthorizedObjectResult("user is unauthorized");
                            }
                            catch (Exception)
                            {
                                context.Result = new UnauthorizedObjectResult("user is unauthorized");
                            }
                        }
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            try
            {
                var statusCode = (int)context.Result.GetType().GetProperty("StatusCode").GetValue(context.Result);
                if (statusCode == 401)
                    context.Result = new UnauthorizedObjectResult("Unauthorized user");
                else if (statusCode == 404)
                    context.Result = new NotFoundObjectResult("Object not found");
                else
                {
                    var allowsAnonymus = ((ControllerActionDescriptor)context.ActionDescriptor).MethodInfo.GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name == "AllowAnonymousAttribute") != null;

                    var actionName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

                    if (!allowsAnonymus || actionName=="Login")
                    {
                        var operationResult = (ObjectResult)context.Result;
                        var castedOperationResult = operationResult.Value;
                        if (castedOperationResult.GetType().Name == "LoginResponse")
                            encryptedRol = castedOperationResult.GetType().GetProperty("Rol").GetValue(castedOperationResult).ToString();

                        if (string.IsNullOrEmpty(encryptedRol))
                            context.Result = new UnauthorizedObjectResult("rol missing");

                        if (!context.Result.GetType().Name.Equals("UnauthorizedResult") && !string.IsNullOrEmpty(encryptedRol))
                        {
                            var decryptedRol = Encryption.Decrypt(encryptedRol).Split(";")[1];
                            if (ActiveUsers.SesionesActivas.Any(x => x.Key.Equals(encryptedRol)))
                            {
                                var token = TokenHelper.GenerateJSONWebToken(configuration, decryptedRol);
                                context.HttpContext.Response.Headers.Add("Token", token);
                                ActiveUsers.SesionesActivas[encryptedRol] = token;
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
