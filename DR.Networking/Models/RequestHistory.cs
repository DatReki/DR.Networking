using System;
using System.Diagnostics;
using static DR.Networking.Core.RateLimiter;

namespace DR.Networking.Models
{
    internal class RequestHistory
    {
        internal RequestHistory() { }

        internal RequestHistory(RequestHistory source) 
        {
            Id = source.Id;
            Url = source.Url;
            Type = source.Type;
            Settings = source.Settings;
            Start = source.Start;
            End = source.End;
        }

        /// <summary>
        /// The unique id of this request.
        /// </summary>
        internal string Id { get; set; } = string.Empty;

        /// <summary>
        /// Url of the request.
        /// </summary>
        internal Uri? Url { get; set; } = null;

        /// <summary>
        /// If the request was finished or not.
        /// </summary>
        internal bool Finished { get; set; } = false;

        /// <summary>
        /// The <see cref="RateLimitType"/> which corresponds to this request.
        /// </summary>
        internal RateLimitType? Type { get; set; } = null;

        /// <summary>
        /// The <see cref="UrlRateLimit"/> which corresponds to this request.
        /// </summary>
        internal UrlRateLimit? Settings { get; set; } = null;

        /// <summary>
        /// The timestamp of when the request was made.
        /// </summary>
        internal long Start { get; set; } = Stopwatch.GetTimestamp();

        /// <summary>
        /// The timestamp of when the request was finished.
        /// </summary>
        internal long? End { get; set; } = null;
    }
}
