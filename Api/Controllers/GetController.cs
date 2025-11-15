using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class GetController : Controller
    {
        [HttpGet]
        public ContentResult Index()
            => Content("This is the result of the example GET request");
    }
}
