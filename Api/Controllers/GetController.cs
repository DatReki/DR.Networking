using Api.Core.Attributes;
using Api.Models;
using Generate;
using Generate.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Api.Controllers
{
    public class GetController : BaseController
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

        [HttpGet]
        public int RandomNumber()
            => Main.RandomNumber();

        [HttpGet]
        public string RandomString()
            => Main.RandomString(0, 10000);

        [HttpGet]
        public string RandomText()
            => Main.RandomText(0, 100);

        [HttpGet]
        public string RandomJson()
            => JsonConvert.SerializeObject(Main.RandomVehicles(0, 50));

        [HttpGet]
        public string RandomXml()
        {
            XDocument xdoc = new(new XDeclaration("1.0", "utf-8", "yes"));

            using (var writer = xdoc.CreateWriter())
            {
                List<Vehicle> vehicles = Main.RandomVehicles(0, 50);
                XmlSerializer x = new(vehicles.GetType());

                x.Serialize(writer, vehicles);
            }

            return xdoc.ToString();
        }
    }
}
