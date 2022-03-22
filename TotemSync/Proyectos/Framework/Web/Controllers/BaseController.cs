using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Framework.Web
{
    public class BaseController: ControllerBase
    {
        private readonly IConfiguration configuration;
        public BaseController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}