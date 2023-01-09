using System.Net.Http;
using System.Net.Http.Headers;

namespace DR.Networking
{
    public class Data
    {
        public bool Result { get; set; }
        public string Error { get; set; } = string.Empty;
        public HttpContent? Content { get; set; } = null;
        public HttpResponseHeaders? Headers { get; set; } = null;
    }
}
