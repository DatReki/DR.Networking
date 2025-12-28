using Api.Core.Extensions;
using Generate.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace Api.Controllers
{
    public class PatchController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(PatchController), HttpStatusCode.OK);

        [HttpPatch]
        public ContentResult EditUser([FromQuery] string email, [FromBody] User user)
        {
            if (string.IsNullOrWhiteSpace(email))
                return GetResult("Email is empty", HttpStatusCode.BadRequest);
            else
                email = HttpUtility.UrlDecode(email);

            User? orgUser = Data.Users.FirstOrDefault(x => x.Email == email);
            if (orgUser == null)
                return GetResult("No user can be found with the provided email", HttpStatusCode.NotFound);

            int index = Data.Users.DeepIndexOf(orgUser);
            if (index == -1)
                return GetResult("No user can be found with the provided email", HttpStatusCode.NotFound);

            Data.Users[index] = user;
            if (!Data.Users[index].DeepEquals(user))
                return GetResult("No user can be found with the provided email", HttpStatusCode.NotFound);

            return GetResult($"User updated '{user.Username}'", HttpStatusCode.OK);
        }
    }
}
