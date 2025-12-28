using Api.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class TraceController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(TraceController), HttpStatusCode.OK);

        [HttpTrace]
        public IActionResult Example()
            => Ok();
    }
}
