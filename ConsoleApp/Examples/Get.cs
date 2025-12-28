using ConsoleApp.Models.Get;
using DR.Networking;
using DR.Networking.Models;
using Intermediate;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp.Examples
{
    internal class Get
    {
        internal static async Task NamedClientString()
        {
            NamedClient client = await Main.CreateClient("test", new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));
            await Program.ShowResult(await ExampleCall("Get", client));
        }

        internal static async Task NamedClientType()
        {
            NamedClient client = await Main.CreateClient<ExampleClass>(new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));
            await Program.ShowResult(await ExampleCall("Get", client));
        }

        internal static async Task ValidIpv4()
        {
            NamedClient client = await Main.CreateClient<ExampleClass>(new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));
            client.Client?.BaseAddress = null;

            await Program.ShowResult(await ExampleCall($"127.0.0.1:{InternalApi.UsedPorts.Last()}/Get", client));
        }

        internal static async Task ValidIpv6()
        {
            NamedClient client = await Main.CreateClient<ExampleClass>(new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));
            client.Client?.BaseAddress = null;

            await Program.ShowResult(await ExampleCall($"[0:0:0:0:0:0:0:1]:{InternalApi.UsedPorts.Last()}/Get", client));
        }

        internal static async Task InavlidUrl()
        {
            NamedClient client = Clients.CreateClient<ExampleClass>(new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            await Program.ShowResult(await ExampleCall("12.4121.134", client));
        }

        internal static async Task NoUrl()
        {
            NamedClient client = Clients.CreateClient<ExampleClass>(new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            await Program.ShowResult(await ExampleCall(string.Empty, client));
        }

        internal static async Task EndpointRateLimiting()
        {
            string clientName = "url-ratelimit";
            TimeSpan limit = TimeSpan.FromMilliseconds(250);
            NamedClient client = await Main.CreateClient(clientName, new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));

            string baseAddress = string.Empty;
            if (client.Client != null && client.Client.BaseAddress != null)
                baseAddress += client.Client.BaseAddress;

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}/Get/RandomNumber"),
                    WholeDomain = false,
                }
            ];

            _ = new Configuration(new ConfigurationOptions()
            {
                NamedClients = [client],
                UrlRateLimits = urlRateLimits,
            });

            Result result = await SendRateLimitRequest(clientName, limit);
            await Program.ShowResult(result);
        }

        internal static async Task DomainRateLimiting()
        {
            TimeSpan limit = TimeSpan.FromMilliseconds(250);
            List<string> endpoints =
            [
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            string clientName = "url-ratelimit";
            NamedClient client = await Main.CreateClient(clientName, new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));

            string baseAddress = string.Empty;
            if (client.Client != null && client.Client.BaseAddress != null)
                baseAddress += client.Client.BaseAddress;

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}"),
                    WholeDomain = true,
                }
            ];

            _ = new Configuration(new ConfigurationOptions()
            {
                NamedClients = [client],
                UrlRateLimits = urlRateLimits,
            });

            Result result = await SendRateLimitRequest(clientName, limit, endpoints, false);
            await Program.ShowResult(result);
        }

        internal static async Task GlobalRateLimiting()
        {
            string clientName = "global-ratelimit";
            TimeSpan limit = TimeSpan.FromMilliseconds(250);

            NamedClient client = await Main.CreateClient(clientName, new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));
            _ = new Configuration(new ConfigurationOptions()
            {
                GlobaRateLimit = limit,
                NamedClients = [client],
            });

            Result result = await SendRateLimitRequest(clientName, limit);
            await Program.ShowResult(result);
        }

        internal static async Task ParallelRateLimiting()
        {
            TimeSpan limit = TimeSpan.FromMilliseconds(30);
            List<string> endpoints =
            [
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            string clientName = "url-ratelimit";
            NamedClient client = await Main.CreateClient(clientName, new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));

            string baseAddress = string.Empty;
            if (client.Client != null && client.Client.BaseAddress != null)
                baseAddress += client.Client.BaseAddress;

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}"),
                    WholeDomain = true,
                }
            ];

            _ = new Configuration(new ConfigurationOptions()
            {
                NamedClients = [client],
                UrlRateLimits = urlRateLimits,
            });

            Result result = await SendRateLimitRequest(clientName, limit, endpoints, true, 500);
            await Program.ShowResult(result);
        }

        private static async Task<Result> SendRateLimitRequest(string clientName, TimeSpan limit, List<string>? endpoints = null, bool parallel = false, int? count = null)
        {
            Result result;
            if (parallel)
            {
                if (count != null)
                    result = await Core.RateLimitRequests.SendParallelRequest(clientName, limit, endpoints, (int)count);
                else
                    result = await Core.RateLimitRequests.SendParallelRequest(clientName, limit, endpoints);
            }
            else
            {
                if (count != null)
                    result = await Core.RateLimitRequests.SendRequest(clientName, limit, count: (int)count);
                else
                    result = await Core.RateLimitRequests.SendRequest(clientName, limit);
            }

            return result;
        }

        internal static async Task<Result> ExampleCall(string url, NamedClient client)
        {
            _ = new Configuration(new ConfigurationOptions()
            {
                NamedClients = [client],
            });
            HttpRequestMessage request = new(HttpMethod.Get, url);

            return await Request.Send(request, client.Name);
        }
    }
}
