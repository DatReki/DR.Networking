using System;
using static DR.Networking.Core.RateLimiter;

namespace DR.Networking.Models
{
    public class UrlRateLimit
    {
        /// <summary>
        /// Url to which you want the rate limit settings to be applied to.
        /// </summary>
        public Uri Uri { get; set; } = new Uri("about:blank");

        /// <summary>
        /// How long to wait before making the next request.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Do you want this setting to be applied to the whole domain? <br />
        /// If false this settings will only be applied to the specific url.
        /// </summary>
        public bool WholeDomain { get; set; } = true;

        /// <summary>
        /// What type of rate limit this is.
        /// </summary>
        internal RateLimitType Type { get; set; } = RateLimitType.Unkown;

        /// <summary>
        /// The string version of the value set by <see cref="Uri"/>.
        /// </summary>
        internal string UrlString { get; set; } = string.Empty;
    }
}
