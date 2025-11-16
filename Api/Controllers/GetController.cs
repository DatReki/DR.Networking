using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class GetController : Controller
    {
        [HttpGet]
        public ContentResult Index()
            => Content("This is the result of the example GET request");

        [HttpGet]
        public List<User> GetUsers()
            => [.. Data.Users];
    }
}
