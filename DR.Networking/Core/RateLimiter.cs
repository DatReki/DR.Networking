using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal class RateLimiter
    {
        internal RateLimiter(string id, long start)
        {
            Id = id;
            Start = start;
        }

        /// <summary>
        /// Id of the request
        /// </summary>
        private readonly string Id;
        
        /// <summary>
        /// Start time of the request
        /// </summary>
        private readonly long Start;

        private static bool GlobalRateLimiting { get { return Settings.GlobalDuration != null && Settings.GlobalDuration.Value.TotalMilliseconds > 0; } }
        private static bool UrlRateLimiting { get { return Settings.UrlRateLimits.Any(x => x.Duration.TotalMilliseconds > 0); } }

        /// <summary>
        /// The different types of ratelimiting
        /// </summary>
        internal enum RateLimitType
        {
            Global,
            Domain,
            Endpoint,
            Unkown,
        }

        /// <summary>
        /// Check if the request needs to be ratelimited. <br />
        /// Will automatically delay the request if it does need to be rate limited.
        /// </summary>
        /// <param name="url">Url associated with the request.</param>
        /// <returns></returns>
        internal async Task<RateLimitResult> Check(Uri? url)
        {
            RateLimitResult result = new();
            if (url == null)
                return result;
            else if (!GlobalRateLimiting && !UrlRateLimiting)
                return result;

            if (!IsRateLimitUrl(url, out RateLimitType type, out UrlRateLimit? settings))
                return result;

            try
            {
                using (CancellationTokenSource cts = new(result.Timeout))
                {
                    while (!IsNextRequest())
                    {
                        // If request is sitting longer in the ratelimit queue than allowed cancel it.
                        if (cts.IsCancellationRequested)
                        {
                            result.TimedOut = true;
                            break;
                        }

                        await Task.Delay(5);
                    }
                }

                if (!result.TimedOut)
                {
                    TimeSpan? waiting = CheckIfRateLimitIsNeeded(type, settings);
                    if (waiting != null)
                    {
                        // If request is sitting longer in the ratelimit queue than allowed cancel it.
                        if (Stopwatch.GetTimestamp() + waiting?.Ticks > Start + result.Timeout.Ticks)
                            result.TimedOut = true;
                        else
                            await Task.Delay(waiting?.Milliseconds ?? 0);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                History.Update(new HistoryData(History.Get(Id))
                {
                    Settings = settings,
                    Type = type,
                });
            }

            return result;
        }

        /// <summary>
        /// Check if the <see cref="Uri"/> we're going to be making a request to even needs ratelimiting.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns>
        /// Returns <see cref="false"/> if the url does not need any rate limiting. <br />
        /// Returns <see cref="true"/> if global rate limit is enabled or an <see cref="UrlRateLimit"/> exists which contains (part of) the provided url.
        /// </returns>
        private static bool IsRateLimitUrl(Uri url, out RateLimitType type, out UrlRateLimit? settings)
        {
            type = RateLimitType.Unkown;
            settings = null;

            if (GlobalRateLimiting && !UrlRateLimiting)
            {
                type = RateLimitType.Global;
                return true;
            }
            else
            {
                if (!Tools.TryGetStartsWith(url, Settings.UrlRateLimits, out List<UrlRateLimit> found))
                {
                    if (GlobalRateLimiting)
                    {
                        type = RateLimitType.Global;
                        return true;
                    }

                    return false;
                }

                settings = found.First();
                if (settings.WholeDomain)
                    type = RateLimitType.Domain;
                else
                    type = RateLimitType.Endpoint;

                return true;
            }
        }

        /// <summary>
        /// Check if the current request is the first in the queue.
        /// </summary>
        /// <returns></returns>
        private bool IsNextRequest()
        {
            HistoryData? inProgress = History.Requests.FirstOrDefault(x => x.End == null);
            if (inProgress != null)
            {
                if (inProgress.Id == Id)
                    return true;
                else
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Check if we even need to delay the request.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static TimeSpan? CheckIfRateLimitIsNeeded(RateLimitType type, UrlRateLimit? settings)
        {
            TimeSpan? result = null;
            HistoryData? previousRequest = null;

            switch (type)
            {
                case RateLimitType.Endpoint:
                    if (settings != null)
                    {
                        previousRequest = History.Requests
                            .LastOrDefault(x => x.Url == settings.Uri && x.End != null);
                    }
                    break;
                case RateLimitType.Domain:
                    if (settings != null)
                    {
                        previousRequest = History.Requests
                            .LastOrDefault(x => x.Url?.Host == settings.Uri.Host && x.End != null);
                    }
                    break;
                case RateLimitType.Global:
                    if (Settings.GlobalDuration != null)
                    {
                        previousRequest = History.Requests
                            .LastOrDefault(x => x.End != null);
                    }
                    break;
            }

            if (previousRequest == null)
                return result;

            // Check if request needs to be rate limited.
            switch (type)
            {
                case RateLimitType.Endpoint:
                case RateLimitType.Domain:
                    if (settings != null)
                    {
                        TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime(previousRequest.End ?? Stopwatch.GetTimestamp());
                        if (timeBetween < settings.Duration)
                            result = settings.Duration.Subtract(timeBetween).RoundUp();
                    }
                    break;
                case RateLimitType.Global:
                    if (Settings.GlobalDuration != null)
                    {
                        TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime(previousRequest.End ?? Stopwatch.GetTimestamp());
                        if (timeBetween < Settings.GlobalDuration)
                            result = ((TimeSpan)Settings.GlobalDuration).Subtract(timeBetween).RoundUp();
                    }
                    break;
            }

            if (result != null && result?.TotalMilliseconds > 0)
                return result;

            return null;
        }
    }
}
