using System;
using static DR.Networking.Core.RateLimiter;

namespace DR.Networking.Models
{
    internal class RequestHistoryItem
    {
        /// <summary>
        /// Url of the request.
        /// </summary>
        internal Uri Url { get; set; } = new Uri("about:blank");

        /// <summary>
        /// The <see cref="RateLimitType"/> which corresponds to this request.
        /// </summary>
        internal RateLimitType Type { get; set; }

        /// <summary>
        /// The <see cref="UrlRateLimit"/> which corresponds to this request.
        /// </summary>
        internal UrlRateLimit? Settings { get; set; } = null;

        /// <summary>
        /// The timestamp of when the request was made.
        /// </summary>
        internal long RequestTimestamp { get; set; }
    }
}
