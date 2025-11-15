namespace Backend.Models
{
    /// <summary>
    /// A model containing the options which can be used when creating a new <see cref="HttpClient"/>
    /// </summary>
    public class HttpClientOptions
    {
        public HttpClientOptions() { }

        public HttpClientOptions(TimeSpan timeout)
        {
            Timeout = timeout;
        }

        public HttpClientOptions(Dictionary<string, string> defaultHeaders, TimeSpan timeout)
        {
            DefaultHeaders = defaultHeaders;
            Timeout = timeout;
        }

        public HttpClientOptions(Dictionary<string, string> defaultHeaders, Version version, HttpVersionPolicy policy, long maxBufferSize, TimeSpan timeout)
        {
            DefaultHeaders = defaultHeaders;
            Version = version;
            Policy = policy;
            MaxBufferSize = maxBufferSize;
            Timeout = timeout;
        }

        public Dictionary<string, string>? DefaultHeaders { get; set; } = null;
        public Version? Version { get; set; } = null;
        public HttpVersionPolicy? Policy { get; set; } = null;
        public long? MaxBufferSize { get; set; } = null;
        public TimeSpan? Timeout { get; set; } = null;
    }
}
