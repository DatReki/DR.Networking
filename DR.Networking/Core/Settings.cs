using DR.Networking.Models;
using System;
using System.Collections.Generic;

namespace DR.Networking.Core
{
    internal class Settings
    {
        /// <summary>
        /// If neither HTTP or HTTPS is provided at the start of a request URL the library will use HTTPS.
        /// </summary>
        internal static bool UseHttpsByDefault { get; set; } = true;

        /// <summary>
        /// If this variable is set it will act as the global duration between requests on the same domain <br /> 
        /// that the library will check for on every request.
        /// </summary>
        internal static TimeSpan? GlobalDuration { get; set; } = null;

        /// <summary>
        /// If this variable is set it will act as the site specific duration between requests on the same domain or url <br />
        /// that the library will check for on every request.
        /// </summary>
        internal static List<UrlSpecificRateLimit>? UrlSpecificRateLimits { get; set; } = null;
    }
}
