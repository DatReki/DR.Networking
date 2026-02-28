using DR.Networking.Core;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DR.Networking
{
    public class RateLimiting
    {
        /// <summary>
        /// Udate the global ratelimit the library uses across all requests.
        /// </summary>
        /// <param name="value"></param>
        public static void UpdateGlobal(TimeSpan? value)
            => Settings.GlobalDuration = value;

        /// <summary>
        /// Update the duration of how long a request can be stuck in the ratelimit queue before it gets cancelled.
        /// </summary>
        public static void UpdateRateLimitTimeout(TimeSpan? value)
            => Settings.RateLimitTimeout = value;

        /// <summary>
        /// Get a list of all the <see cref="UrlRateLimit"/>'s that have been added to the library.
        /// </summary>
        public static List<UrlRateLimit> GetUrlRateLimits()
            => [.. Settings.UrlRateLimits];

        /// <summary>
        /// Try to add multiple url specific rate limits to the library.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// Will return <see cref="true"/> when any <see cref="UrlRateLimit"/> can be added.<br />
        /// Will return <see cref="false"/> when none can be added (already added).
        /// </returns>
        public static bool Add(List<UrlRateLimit> value)
        {
            bool result = false;
            foreach (UrlRateLimit item in value)
            {
                bool added = Add(item);
                if (!result && added)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Try to add a single url specific rate limit to the library.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>
        /// Will return <see cref="true"/> when the <see cref="UrlRateLimit"/> has been added.<br />
        /// Will return <see cref="false"/> when it cannot be added (already added).
        /// </returns>
        public static bool Add(UrlRateLimit value)
        {
            UrlRateLimit? created = CreateUrlRatelimit(value);
            if (created == null)
                return false;

            if (!Settings.UrlRateLimits.Any(x => x.Uri == value.Uri))
            {
                Settings.UrlRateLimits.Add(value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try and remove multiple <see cref="UrlRateLimit"/> from the library.
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see cref="true"/> if any rate limit has been removed otherwise <see cref="false"/>.</returns>
        public static bool Remove(List<UrlRateLimit> value)
        {
            bool result = false;
            foreach (UrlRateLimit item in value)
            {
                bool added = Remove(item);
                if (!result && added)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Try and remove a <see cref="UrlRateLimit"/> from the library.
        /// </summary>
        /// <param name="value"></param>
        /// <returns><see cref="true"/> if the rate limit has been removed otherwise <see cref="false"/>.</returns>
        public static bool Remove(UrlRateLimit value)
        {
            UrlRateLimit? created = CreateUrlRatelimit(value);
            if (created == null)
                return false;

            UrlRateLimit? found = Settings.UrlRateLimits.FirstOrDefault(x => x == value);
            if (found != null)
            {
                Settings.UrlRateLimits.Remove(found);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Go through the required steps to create a valid <see cref="UrlRateLimit"/> object which can be used by the library.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static UrlRateLimit? CreateUrlRatelimit(UrlRateLimit value)
        {
            value.Uri = value.Uri.RemoveDoubleSlashes();
            return value;
        }
    }
}
