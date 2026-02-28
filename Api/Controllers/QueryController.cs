using Api.Core.Attributes;
using Generate.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace Api.Controllers
{
    public class QueryController : BaseController
    {
        [HttpGet]
        public ContentResult Index()
            => GetResult(nameof(QueryController), HttpStatusCode.OK);

        [HttpQuery]
        public List<User> GetUsersByEmail([FromBody] List<string> emails)
        {
            if (emails.Count == 0)
                return [];
            else
                emails = [.. emails.Select(x => HttpUtility.UrlDecode(x))];

            return [.. Data.Users.Where(x => emails.Contains(x.Email))];
        }
    }
}
