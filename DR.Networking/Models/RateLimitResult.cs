using DR.Networking.Core;
using System;

namespace DR.Networking.Models
{
    internal class RateLimitResult
    {
        /// <summary>
        /// How long a request can be stuck in the ratelimit queue before it gets cancelled.
        /// </summary>
        internal TimeSpan Timeout {  get; set; } = Settings.RateLimitTimeout ?? TimeSpan.FromMilliseconds(int.MaxValue);

        /// <summary>
        /// Indicates if the request was in the ratelimit queue for more than the specified <see cref="Timeout"/> duration and got cancelled.
        /// </summary>
        internal bool TimedOut { get; set; } = false;
    }
}
