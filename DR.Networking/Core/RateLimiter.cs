using DR.ConcurrentCollections;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal class RateLimiter
    {
        private static readonly ConcurrentObservableCollection<RequestHistoryItem> RequestHistory = [];
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
        internal static async Task Check(Uri? url)
        {
            if (url == null)
                return;
            else if (!GlobalRateLimiting && !UrlRateLimiting)
                return;

            if (!IsRateLimitUrl(url, out RateLimitType type, out UrlRateLimit? settings))
                return;

            try
            {
                double? waiting = CheckIfRateLimitIsNeeded(type, settings);
                if (waiting == null)
                    return;
                else
                    await Task.Delay((int)waiting);
            }
            catch
            {
                throw;
            }
            finally
            {
                RequestHistory.Add(new RequestHistoryItem()
                {
                    RequestTimestamp = Stopwatch.GetTimestamp(),
                    Settings = settings,
                    Type = type,
                    Url = url,
                });
            }

            return;
        }

        internal static void ListenToChanges()
        {
            RequestHistory.CollectionChanged -= RequestHistoryChange;
            RequestHistory.CollectionChanged += RequestHistoryChange;
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
        /// Check if we even need to delay the request.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static double? CheckIfRateLimitIsNeeded(RateLimitType type, UrlRateLimit? settings)
        {
            double? result = null;
            RequestHistoryItem? previousRequest = null;

            switch (type)
            {
                case RateLimitType.Endpoint:
                    if (settings != null)
                    {
                        previousRequest = RequestHistory
                            .Where(x => x.Url == settings.Uri)
                            .LastOrDefault();
                    }
                    break;
                case RateLimitType.Domain:
                    if (settings != null)
                    {
                        previousRequest = RequestHistory
                            .Where(x => x.Url.Host == settings.Uri.Host)
                            .LastOrDefault();
                    }
                    break;
                case RateLimitType.Global:
                    if (Settings.GlobalDuration != null)
                    {
                        previousRequest = RequestHistory.LastOrDefault();
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
                        TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime(previousRequest.RequestTimestamp);
                        if (timeBetween < settings.Duration)
                            result = settings.Duration.Subtract(timeBetween).TotalMilliseconds.RoundUp();
                    }
                    break;
                case RateLimitType.Global:
                    if (Settings.GlobalDuration != null)
                    {
                        TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime(previousRequest.RequestTimestamp);
                        if (timeBetween < Settings.GlobalDuration)
                            result = ((TimeSpan)Settings.GlobalDuration).Subtract(timeBetween).TotalMilliseconds.RoundUp();
                    }
                    break;
            }

            if (result != null && result > 0)
                return result;

            return null;
        }

        /// <summary>
        /// Remove old items from the list if they're outside of their rate limit durations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RequestHistoryChange(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            else if (e.NewItems == null)
                return;

            // For performance reasons only remove items if there are more than 100 requests in the list.
            if (RequestHistory.Count > 100)
            {
                long timestamp = Stopwatch.GetTimestamp();
                IEnumerable<RequestHistoryItem> remove = [.. RequestHistory.Where(x =>
                {
                    TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime(x.RequestTimestamp, timestamp);
                    if (x.Type == RateLimitType.Endpoint || x.Type == RateLimitType.Endpoint)
                    {
                        if (x.Settings == null)
                            return true;
                        else if (timeBetween > x.Settings.Duration)
                            return true;
                        else
                            return false;
                    }
                    else if (x.Type == RateLimitType.Global)
                    {
                        if (timeBetween > Settings.GlobalDuration)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                })];

                foreach (RequestHistoryItem item in remove)
                    RequestHistory.Remove(item);
            }
        }
    }
}
