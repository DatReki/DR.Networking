using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class BaseController : Controller
    {
        protected virtual ContentResult GetResult(string message, HttpStatusCode status)
        {
            return new ContentResult()
            {
                Content = message,
                ContentType = "text/plain",
                StatusCode = (int)status
            };
        }
    }
}
