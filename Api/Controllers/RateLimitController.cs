using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Net;

namespace Api.Controllers
{
    [EnableRateLimiting("ratelimit")]
    public class RateLimitController : BaseController
    {
        [HttpGet]
        [DisableRateLimiting]
        public ContentResult Index()
            => GetResult(nameof(RateLimitController), HttpStatusCode.OK);

        [HttpGet]
        public ContentResult Basic()
            => GetResult("Not ratelimited", HttpStatusCode.OK);
    }
}
