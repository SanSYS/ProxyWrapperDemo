using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProxyWrapper;
using ProxyWrapper.Contract;

namespace ProxyWrapperWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController: ControllerBase
    {
        private readonly IProxyWrapperStorage _wrapperService;

        public ProxyController(IProxyWrapperStorage wrapperService)
        {
            _wrapperService = wrapperService;
        }

        [HttpGet("interfaces")]
        public Task<IEnumerable<Interface>> GetInterfaces()
        {
            return _wrapperService.GetInterfaces();
        }

        [HttpGet("methods")]
        public Task<IEnumerable<ServiceMethodInfo>> GetServiceMethods(string service)
        {
            byte[] res = Convert.FromBase64String(service);
            string className = Encoding.UTF8.GetString(res);

            return _wrapperService.GetServiceMethods(className);
        }

        [HttpPost("save")]
        public Task Save(ServiceMethodInfo info)
        {
            return _wrapperService.Save(info);
        }
    }
}
