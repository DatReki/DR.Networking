using System.Net.Http;
using System.Net.Http.Headers;

namespace DR.Networking.Models
{
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
        /// The HTTP status code associated with the request. <br />
        /// Will return -1 if the url used for the request is invalid.
        /// </summary>
        public int StatusCode { get; set; } = -1;

        /// <summary>
        /// The error explaining what went wrong during the request if <see cref="Success"/> is false.
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// The response content of the request if <see cref="Success"/> is true.
        /// </summary>
        public HttpContent? Content { get; set; } = null;

        /// <summary>
        /// The response headers of the request.
        /// </summary>
        public HttpResponseHeaders? Headers { get; set; } = null;

    }
}
