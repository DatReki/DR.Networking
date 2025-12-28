using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;
using static DR.Networking.Core.RateLimiter;

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
        /// Enable or disable the libraries url validation.
        /// </summary>
        internal static bool ValidateUrl { get; set; } = true;

        /// <summary>
        /// Create a clone of the <see cref="HttpRequestMessage"/> send by the library (can be useful for debugging and logging).
        /// </summary>
        internal static bool CloneRequestMessage { get; set; } = true;

        /// <summary>
        /// If this variable is set it will act as the site specific duration between requests on the same domain or url <br />
        /// that the library will check for on every request.
        /// </summary>
        internal static ObservableCollection<UrlRateLimit> UrlRateLimits { get; set; } = new ObservableCollection<UrlRateLimit>();

        /// <summary>
        /// The HttpClient used to make the requests.
        /// </summary>
        internal static HttpClient Client = new HttpClient(new StandardSocketsHttpHandler()
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            PooledConnectionLifetime = TimeSpan.FromMinutes(1),
        });

        /// <summary>
        /// Allow users to pass custom HttpClients used for specific requests.
        /// </summary>
        internal static List<NamedClient> NamedClients = new List<NamedClient>();

        internal static void ListenToChanges()
        {
            UrlRateLimits.CollectionChanged -= UrlRateLimitChange;
            UrlRateLimits.CollectionChanged += UrlRateLimitChange;
        }

        private static void UrlRateLimitChange(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;
            else if (e.NewItems == null)
                return;

            foreach (UrlRateLimit item in e.NewItems)
            {
                if (string.IsNullOrWhiteSpace(item.UrlString))
                    item.UrlString = item.Uri.ToString();

                if (item.Type == RateLimitType.Unkown)
                {
                    if (item.WholeDomain)
                        item.Type = RateLimitType.Domain;
                    else
                        item.Type = RateLimitType.Endpoint;
                }
            }
        }
    }
}
