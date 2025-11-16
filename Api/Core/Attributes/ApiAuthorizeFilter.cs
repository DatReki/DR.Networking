using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Api.Core.Attributes
{
    public class ApiAuthorizeFilter(IHttpContextAccessor httpContextAccessor) : IAuthorizationFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            void Result(string ip, bool authorized, string message, HttpStatusCode status)
            {
                context.Result = new UnauthorizedObjectResult(string.Empty)
                {
                    StatusCode = (int)status,
                    Value = new AuthorizedResponse<string>()
                    {
                        Ip = ip,
                        Authorized = authorized,
                        Message = message,
                        StatusCode = status,
                        Content = null,
                    }
                };
            }

            HttpContext httpContext = _httpContextAccessor.HttpContext!;
            string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

#if !DEBUG
            if (string.IsNullOrWhiteSpace(ip))
            {
                Result(ip, false, "Cannot find the IP", HttpStatusCode.Unauthorized);
                return;
            }
#endif

            if (!httpContext.Request.Headers.TryGetHeaderValue("X-Request-ID", out string id))
            {
                Result(ip, false, "No 'Id' header provided", HttpStatusCode.Unauthorized);
                return;
            }

            if (!httpContext.Request.Headers.TryGetHeaderValue("Authorization", out string auth))
            {
                Result(ip, false, "No 'Authorization' header provided", HttpStatusCode.Unauthorized);
                return;
            }

            ApiUser? user = Data.ApiUsers.FirstOrDefault(x => x.ClientId.ToString() == id && x.ClientSecret == auth);
            if (user == null)
            {
                Result(ip, false, "No user with the provided 'ClientId' & 'ClientSecret' could be found", HttpStatusCode.Unauthorized);
                return;
            }
        }
    }
}
