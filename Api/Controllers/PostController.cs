using Generate.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class PostController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(PostController), HttpStatusCode.OK);

        [HttpPost]
        public ContentResult CreateUser([FromBody] User user)
        {
            if (user == null)
                return GetResult("No user provided", HttpStatusCode.BadRequest);
            else
                Data.Users.Add(user);

            return GetResult($"User added '{user.Username}'", HttpStatusCode.Created);
        }
    }
}
