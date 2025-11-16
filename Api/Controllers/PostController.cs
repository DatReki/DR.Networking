using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class PostController : Controller
    {
        public ContentResult Index()
            => GetResult("Post controller", HttpStatusCode.OK);

        [HttpPost]
        public ContentResult CreateUser([FromBody] User user)
        {
            if (user == null)
                return GetResult("No user provided", HttpStatusCode.BadRequest);
            else
                Data.Users.Add(user);

            return GetResult($"User added '{user.Username}'", HttpStatusCode.OK);
        }

        private static ContentResult GetResult(string message, HttpStatusCode status)
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
