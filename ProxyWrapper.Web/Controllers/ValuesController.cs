using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProxyWrapper.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly IProxyWrapperStorage _wrapperService;

        public ProxyController(IProxyWrapperStorage wrapperService)
        {
            _wrapperService = wrapperService;
        }
        
        // GET api/values
        [HttpGet("interfaces")]
        public Task<IEnumerable<Interface>> GetInterfaces()
        {
            return _wrapperService.GetInterfaces();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return "from post";
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
