using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class HeadController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(HeadController), HttpStatusCode.OK);

        [HttpHead]
        public IActionResult Example()
            => Ok();
    }
}
