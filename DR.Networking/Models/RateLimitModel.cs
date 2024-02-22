using System;

namespace DR.Networking.Models
{
    internal class RateLimitModel
    {
        /// <summary>
        /// Url of the request.
        /// </summary>
        internal string Url { get; set; } = string.Empty;

        /// <summary>
        /// Date & time the request was made.
        /// </summary>
        internal DateTime RequestDate { get; set; }
    }
}
