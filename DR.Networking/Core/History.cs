using DR.ConcurrentCollections;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace DR.Networking.Core
{
    internal class History
    {
        /// <summary>
        /// A list of requests made using the library (old requests are automatically cleaned up)
        /// </summary>
        internal static readonly ConcurrentObservableCollection<RequestHistory> Requests = [];

        /// <summary>
        /// A list of urls that have been checked using <see cref="Base.CheckUrl(string)"/> function (old urls are automatically cleaned up).
        /// </summary>
        internal static readonly ConcurrentObservableCollection<UrlHistory> Urls = [];

        internal static void AddRequest(RequestHistory data)
            => Requests.Add(data);

        internal static RequestHistory GetRequest(string id)
            => Requests.First(x => x.Id == id);

        internal static void UpdateRequest(string id, RequestHistory data)
        {
            lock (Requests)
            {
                RequestHistory? request = Requests.Find(x => x.Id == id);
                if (request == null)
                    return;

                Requests[Requests.IndexOf(request)] = data;
            }
        }

        internal static void ListenToChanges()
        {
            Requests.CollectionChanged -= RequestHistoryChanged;
            Requests.CollectionChanged += RequestHistoryChanged;

            Urls.CollectionChanged -= UrlHistoryChanged;
            Urls.CollectionChanged += UrlHistoryChanged;

            Timer timer = new(TimeSpan.FromHours(1).TotalMilliseconds);
            timer.Elapsed -= CheckUrlHistory;
            timer.Elapsed += CheckUrlHistory;
            timer.Start();
        }

        /// <summary>
        /// Remove old items from the list if they're outside of their rate limit durations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void RequestHistoryChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            else if (e.NewItems == null)
                return;

            // For performance reasons only remove items if there are more than 100 requests in the list.
            if (Requests.Count > 100)
            {
                long timestamp = Stopwatch.GetTimestamp();
                IEnumerable<RequestHistory> remove = [.. Requests.Where(x =>
                {
                    if (x.End == null)
                        return false;

                    TimeSpan timeBetween = Tools.Stopwatch.GetElapsedTime((long)x.End, timestamp);
                    if (x.Type == RateLimiter.RateLimitType.Endpoint || x.Type == RateLimiter.RateLimitType.Endpoint)
                    {
                        if (x.Settings == null)
                            return true;
                        else if (timeBetween > x.Settings.Duration)
                            return true;
                        else
                            return false;
                    }
                    else if (x.Type == RateLimiter.RateLimitType.Global)
                    {
                        if (timeBetween > Settings.GlobalDuration)
                            return true;
                        else
                            return false;
                    }
                    else
                        return true;
                })];

                foreach (RequestHistory item in remove)
                    Requests.Remove(item);
            }
        }

        /// <summary>
        /// Remove old urls from the list if they haven't been checked for too long.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CheckUrlHistory(object? sender, ElapsedEventArgs e)
        {
            long currentTime = Stopwatch.GetTimestamp();
            IEnumerable<UrlHistory> remove = Urls.Where(x => Tools.Stopwatch.GetElapsedTime(x.Timestamp, currentTime) > TimeSpan.FromHours(1));
            Urls.RemoveAll(x => remove.Contains(x));
        }

        /// <summary>
        /// Make sure that the url history list doesn't get too long.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void UrlHistoryChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            else if (e.NewItems == null)
                return;

            // For performance we're going to make sure that the url history list doesn't get too long by removing the oldest 100 urls if there are more than 1000 urls in the list.
            if (Urls.Count > 1000)
            {
                IEnumerable<UrlHistory> remove = Urls.OrderBy(x => x.Timestamp).Take(100);
                Urls.RemoveAll(x => remove.Contains(x));
            }
        }
    }
}
