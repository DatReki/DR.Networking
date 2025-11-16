using Api.Core.Attributes;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

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

        [HttpGet]
        public User? GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            else
                email = HttpUtility.UrlDecode(email);

            return Data.Users.FirstOrDefault(x => x.Email == email);
        }

        [HttpGet]
        [ApiAuthorize]
        public AuthorizedResponse<User> GetUserAuthorized([FromQuery] string email)
        {
            AuthorizedResponse<User> Result(bool authorized, string message, HttpStatusCode status, User? user = null)
            {
                if (authorized)
                {
                    user = Data.Users.FirstOrDefault(x => x.Email == email);
                    if (user == null)
                        message = "The provided user could not be found";
                }
                else
                    user = null;

                return new AuthorizedResponse<User>()
                {
                    Ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                    Authorized = authorized,
                    Message = message,
                    StatusCode = status,
                    Content = user,
                };
            }

            if (string.IsNullOrWhiteSpace(email))
                return Result(false, "The email parameter is empty", HttpStatusCode.BadRequest);
            else
                email = HttpUtility.UrlDecode(email);

            return Result(true, string.Empty, HttpStatusCode.BadRequest);
        }
    }
}
