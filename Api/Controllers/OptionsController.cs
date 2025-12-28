using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class OptionsController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(OptionsController), HttpStatusCode.OK);

        [HttpOptions]
        public IActionResult Example()
            => Ok();
    }
}
