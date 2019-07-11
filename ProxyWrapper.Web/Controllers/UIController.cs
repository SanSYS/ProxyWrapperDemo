using Microsoft.AspNetCore.Mvc;

namespace ProxyWrapper.Web.Controllers
{
    [Route("UI")]
    [ApiController]
    public class UIController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get()
        {
            return new FileContentResult(System.IO.File.ReadAllBytes("wwwroot/index.html"), "text/html");
        }
    }
}
