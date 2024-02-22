using DR.Networking.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal class RateLimiter
    {
        private static readonly ObservableCollection<RateLimitModel> RequestHistory = new ObservableCollection<RateLimitModel>();

        public RateLimiter()
        {
            RequestHistory.CollectionChanged += RequestHistoryChange;
        }

        /// <summary>
        /// Check if the request needs to be ratelimited. <br />
        /// Will automatically delay the request if it does need to be rate limited.
        /// </summary>
        /// <param name="url">Url associated with the request.</param>
        /// <returns></returns>
        internal static async Task Check(string url)
        {
            int? rateLimitTime = CheckIfRateLimitIsNeeded(url);

            if (rateLimitTime == null)
                return;
            else
                await Task.Delay((int)rateLimitTime);

            return;
        }

        /// <summary>
        /// Check if we even need to delay the request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static int? CheckIfRateLimitIsNeeded(string url)
        {
            var previousRequest = RequestHistory
                .Where(x => x.Url == url)
                .FirstOrDefault();

            if (previousRequest != null)
            {
                UrlSpecificRateLimit? urlSpecific = CheckUrlSpecificRateLimits(url, false);
                urlSpecific ??= CheckUrlSpecificRateLimits(url, true);

                // Check if request needs to be rate limited.
                if (urlSpecific != null)
                {
                    TimeSpan timeBetweenPreviousRequest = previousRequest.RequestDate - DateTime.Now;

                    if (timeBetweenPreviousRequest < urlSpecific.Duration)
                        return (timeBetweenPreviousRequest - urlSpecific.Duration).TotalMilliseconds.RoundUp();
                }
                else if (Settings.GlobalDuration != null)
                {
                    TimeSpan timeBetweenPreviousRequest = previousRequest.RequestDate - DateTime.Now;

                    if (timeBetweenPreviousRequest < Settings.GlobalDuration)
                        return (timeBetweenPreviousRequest - (TimeSpan)Settings.GlobalDuration).TotalMilliseconds.RoundUp();
                }
            }

            return null;
        }

        /// <summary>
        /// Remove old items from the list if they're outside of their rate limit durations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RequestHistoryChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            foreach (RateLimitModel item in e.OldItems)
            {
                UrlSpecificRateLimit? urlSpecific = CheckUrlSpecificRateLimits(item.Url, false);
                urlSpecific ??= CheckUrlSpecificRateLimits(item.Url, true);

                // Remove any items from collection that are passed the ratelimit duration.
                if (urlSpecific != null && (item.RequestDate - DateTime.Now) > urlSpecific.Duration)
                    RequestHistory.Remove(item);
                else if (Settings.GlobalDuration != null && (item.RequestDate - DateTime.Now) > Settings.GlobalDuration)
                    RequestHistory.Remove(item);
            }
        }

        /// <summary>
        /// Check if url is contained in the <see cref="Settings.UrlSpecificRateLimits"/> list.
        /// </summary>
        /// <param name="url">Url for which you want to check if it's in the list.</param>
        /// <param name="wholeDomain">If you want to check if the domain of the url is in the list.</param>
        /// <returns></returns>
        private static UrlSpecificRateLimit? CheckUrlSpecificRateLimits(string url, bool wholeDomain)
        {
            if (wholeDomain)
            {
                return Settings.UrlSpecificRateLimits
                    .Where(x => x.Url == url)
                    .FirstOrDefault();
            }

            return Settings.UrlSpecificRateLimits
                .Where(x => x.WholeDomain && url.Contains(x.Url))
                .FirstOrDefault();
        }
    }
}
