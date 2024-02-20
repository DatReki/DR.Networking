using System;

namespace DR.Networking.Models
{
    public class UrlSpecificRateLimiting
    {
        /// <summary>
        /// Url to which you want the rate limit settings to be applied to.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// How long to wait before making the next request.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Do you want this setting to be applied to the whole domain? <br />
        /// If false this settings will only be applied to the specific url.
        /// </summary>
        public bool WholeDomain { get; set; } = true;
    }
}
