using Api.Core.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class ConnectController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(ConnectController), HttpStatusCode.OK);

        [HttpConnect]
        public IActionResult Example()
            => Ok();
    }
}
