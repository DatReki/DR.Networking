using DR.ConcurrentCollections;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace DR.Networking.Core
{
    internal class History
    {
        /// <summary>
        /// A list of requests made using the library (old requests are automatically cleaned up)
        /// </summary>
        internal static readonly ConcurrentObservableCollection<HistoryData> Requests = [];

        internal static void Add(HistoryData data)
            => Requests.Add(data);

        internal static HistoryData Get(string id)
            => Requests.First(x => x.Id == id);

        internal static void Update(HistoryData data)
        {
            HistoryData request = Requests.First(x => x.Id == data.Id);
            Requests[Requests.IndexOf(request)] = data;
        }

        internal static void ListenToChanges()
        {
            Requests.CollectionChanged -= CollectionChanged;
            Requests.CollectionChanged += CollectionChanged;
        }

        /// <summary>
        /// Remove old items from the list if they're outside of their rate limit durations.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            else if (e.NewItems == null)
                return;

            // For performance reasons only remove items if there are more than 100 requests in the list.
            if (Requests.Count > 100)
            {
                long timestamp = Stopwatch.GetTimestamp();
                IEnumerable<HistoryData> remove = [.. Requests.Where(x =>
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

                foreach (HistoryData item in remove)
                    Requests.Remove(item);
            }
        }
    }
}
