using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("[action]")]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
