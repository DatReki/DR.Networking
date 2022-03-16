using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

using static DRN_Console.Global;

namespace DRN_Console.RunRequests
{
    internal class WithRateLimiting
    {
        internal static void Get()
        {
            ColoredConsole("Running an example get request with ratelimiting");
            GetExample();
        }

        private static void GetExample()
        {
            TimeSpan globalRateLimit = TimeSpan.FromSeconds(10);
            List<DR.Networking.Configuration.SiteSpecific> rateLimitings = new()
            {
                new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(5), Url = "https://example.com/" },
                new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(10), Url = "https://example.org/" },
                new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(15), Url = "https://example.net/" },
                new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(20), Url = "https://example.edu/" },
                new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(25), Url = "https://example.nl/by-sidn/" },
            };

            _ = new DR.Networking.Configuration(globalRateLimit, rateLimitings);

            List<string> Urls = new()
            {
                "https://example.com/",
                "https://example.com/",
                "https://example.org/",
                "https://example.org/",
                "https://example.net/",
                "https://example.net/",
                "https://example.edu/",
                "https://example.edu/",
                "https://example.nl/by-sidn/",
                "https://example.nl/by-sidn/",
            };

            DateTime start = DateTime.UtcNow;

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = new();
            DateTime now;
            DateTime end;

            foreach (string url in Urls)
            {
                now = DateTime.UtcNow;

                result = DR.Networking.Request.Get(url).Result;

                end = DateTime.UtcNow;
                ColoredConsole($"Request url: {url}\nRequest status: {result.result}\nRequest start: {now}\nRequest end: {end}\nTime difference: {(end - now).TotalSeconds}\n\n\n\n");
            }
            DateTime finish = DateTime.UtcNow;

            int totalTimeExpected = 0;
            rateLimitings.ForEach(x => totalTimeExpected += x.Duration.Seconds);

            ColoredConsole($"Request done. Expected around {totalTimeExpected} seconds. Result: {(finish - start).TotalSeconds}s\n\n\n\n");
        }
    }
}
