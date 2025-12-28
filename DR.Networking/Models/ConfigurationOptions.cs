using System;
using System.Collections.Generic;
using System.Net.Http;

namespace DR.Networking.Models
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions()
        {

        }

        /// <summary>
        /// A global ratelimit applied to all request. <br /> 
        /// This value will be ignored if the url already exists in a <see cref="UrlRateLimit"/>.
        /// </summary>
        public TimeSpan? GlobaRateLimit { get; set; } = null;

        /// <summary>
        /// A list of url/domain specific rate limit settings.
        /// </summary>
        public List<UrlRateLimit>? UrlRateLimits { get; set; } = null;

        /// <summary>
        /// Pass your own <see cref="HttpClient"/> for the library to use when no <see cref="NamedClient"/> is provided/found.<br />
        /// Will use the default <see cref="HttpClient"/> if none are provided.
        /// </summary>
        public HttpClient? BaseClient { get; set; } = null;

        /// <summary>
        /// A list of HttpClients the library can use for requests to different endpoints/domains.
        /// </summary>
        public List<NamedClient>? NamedClients { get; set; } = null;

        /// <summary>
        /// If no HTTP or HTTPS is provided at the start of a url the library will automatically use HTTPS if this value is set to <see cref="true"/> (the default value).<br />
        /// If set to <see cref="false"/> the library will add HTTP infront of a url if no HTTP or HTTPS is provided.
        /// </summary>
        public bool UseHttpsByDefault { get; set; } = true;

        /// <summary>
        /// Enable or disable the libraries url validation.
        /// </summary>
        public bool ValidateUrl { get; set; } = true;

        /// <summary>
        /// Create a clone of the <see cref="HttpRequestMessage"/> send by the library (can be useful for debugging and logging).
        /// </summary>
        public bool CloneRequestMessage { get; set; } = false;
    }
}
