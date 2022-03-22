using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Helpers
{
    /// <summary>
    /// Operation filter to add the requirement of the custom header
    /// </summary>
    public class HeaderFilterForRole : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            try
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                var noAuthRequired = context.ApiDescription.CustomAttributes().Any(attr => attr.GetType() == typeof(AllowAnonymousAttribute));
                if (!noAuthRequired)
                {
                    operation.Parameters.Add(new OpenApiParameter()
                    {
                        Name = "rol",
                        In = ParameterLocation.Header,
                        Required = true,
                        AllowEmptyValue = false,
                        Schema = new OpenApiSchema
                        {
                            Type = "string",
                            Default = new OpenApiString("")
                        }
                    }); ;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
