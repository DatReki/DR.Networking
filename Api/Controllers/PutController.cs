using Generate.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Controllers
{
    public class PutController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(PutController), HttpStatusCode.OK);

        [HttpPut]
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
