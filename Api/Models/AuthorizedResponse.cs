using System.Net;

namespace Api.Models
{
    public class AuthorizedResponse<T>
    {
        public string Ip { get; set; } = string.Empty;
        public bool Authorized { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; }
        public required T? Content { get; set; }
    }
}
