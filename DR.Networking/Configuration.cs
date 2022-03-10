using DR.Networking.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DR.Networking
{
    public class Configuration
    {
        public Configuration(TimeSpan rateLimit, List<SiteSpecific> perSite = null)
        {
            Global = rateLimit;
            if (perSite != null)
                PerSite = perSite;
            Core.Base.UpdateSiteSpecificList(PerSite);
        }

        static Configuration()
        {
            s_requestCollection.CollectionChanged += RequestCollectionChanged;
        }

        //Global rate limit cool down
        public static TimeSpan? Global { get; set; }

#nullable enable
        //Rate limit based on individual domains/urls
        public static List<SiteSpecific>? PerSite { get; set; }
#nullable disable

        internal static ObservableCollection<RequestData> s_requestCollection = new ObservableCollection<RequestData>();

        public partial class SiteSpecific
        {
            public string Url { get; set; }
            public TimeSpan Duration { get; set; }
            internal Core.Base.UrlType? _urlType { get; set; }
            internal Uri _uri { get; set; }
        }

        internal partial class RequestData
        {
            internal Uri _url { get; set; }
            internal DateTime _time { get; set; }
        }

        /// <summary>
        /// Check if data about previous requests (Uri and time of the request) can be removed from the collection. 
        /// This is for rate limiting purposes
        /// </summary>
        /// <param name="sender">The object that raised the event</param>
        /// <param name="e">Information about the event</param>
        private static void RequestCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            for (int i = 0; i < s_requestCollection.Count; i++)
            {
                RequestData item = s_requestCollection[i];

                (bool result, SiteSpecific item) found = PerSite.FindUri(item._url);
                if (found.result)
                {
                    RequestCollectionRemove(found.item.Duration, (Core.Base.UrlType)found.item._urlType, item);
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Remove item from collection after the ratelimit time span has passed
        /// </summary>
        /// <param name="rateLimit">Duration for how long you want to wait between requests</param>
        /// <param name="type">Type of the url that's been checked (since we can have both domain and page specific ratelimits).</param>
        /// <param name="item">The item that is to be removed from the collection</param>
        private static void RequestCollectionRemove(TimeSpan rateLimit, Core.Base.UrlType type, RequestData item)
        {
            bool check = (DateTime.UtcNow - item._time) > rateLimit;
            //Console.WriteLine($"Current time: {DateTime.UtcNow}\n" +
            //    $"Request was made at: {item._time}\n" +
            //    $"Timespan for ratelimit is: {rateLimit}\n" +
            //    $"Check: {check}");

            if (check)
            {
                switch (type)
                {
                    case Core.Base.UrlType.Page:
                        s_requestCollection.Remove(item);
                        break;
                    case Core.Base.UrlType.Domain:
                        item = s_requestCollection.Where(i => i._url.AbsolutePath == item._url.AbsolutePath 
                            && i._time == item._time).FirstOrDefault();
                        s_requestCollection.Remove(item);
                        break;
                }
            }
        }
    }
}
