using System.Net.Http;
using System.Net.Http.Headers;

namespace DR.Networking.Models
{
    /// <summary>
    /// Indicates what type of error the error message is referencing.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// The library was able to parse either a valid IPv4/IPv6 address from the URL or a fully qualified domain name (FQDN).
        /// </summary>
        None,

        /// <summary>
        /// The library was unable to parse either an IPv4 or IPv6 address from the URL.
        /// </summary>
        InvalidIpAddress,

        /// <summary>
        /// The library was unable to parse a fully qualified domain name (FQDN) from the URL.
        /// </summary>
        InvalidDomain,

        /// <summary>
        /// The hostname was invalid or the library was either unable to resolve or parse it.
        /// </summary>
        InvalidHostname,

        /// <summary>
        /// The library was unable to parse either a valid IPv4/IPv6 address from the URL or a fully qualified domain name (FQDN).
        /// </summary>
        InvalidUrl,

        /// <summary>
        /// The <see cref="HttpMethod"/> is either not supported or not yet implemented
        /// </summary>
        HttpMethodNotSupported,
    }

    /// <summary>
    /// Contains the data about the result of the request.
    /// </summary>
    public class ResultData
    {
        /// <summary>
        /// Indicates if the request was successful.
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// The url to which the request was made.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The HTTP status code associated with the request. <br />
        /// Will return -1 if the url used for the request is invalid.
        /// </summary>
        public int StatusCode { get; set; } = -1;

        /// <summary>
        /// The error explaining what went wrong during the request if <see cref="Success"/> is false.
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Indicates what type of error the error message is referencing.
        /// </summary>
        public ErrorType ErrorType { get; set; }

        /// <summary>
        /// The full HTTP request message.
        /// </summary>
        public HttpRequestMessage? Request { get; set; } = null;

        /// <summary>
        /// The full HTTP response message if <see cref="Success"/> is true.
        /// </summary>
        public HttpResponseMessage? Response { get; set; } = null;

        /// <summary>
        /// The response content of the request if <see cref="Success"/> is true.
        /// </summary>
        public HttpContent? Content { get; set; } = null;

    }
}
