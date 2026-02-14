using DR.Networking;
using DR.Networking.Models;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Tests.Requests
{
    [TestFixture]
    internal class RateLimiting
    {
        private static NamedClient? EndpointClient { get; set; } = null;
        private static NamedClient? DomainClient { get; set; } = null;
        private static NamedClient? GlobalClient { get; set; } = null;

        [OneTimeSetUp]
        public async Task Setup()
        {
            EndpointClient = await Intermediate.Main.CreateClient("EndpointClient", new Intermediate.Models.HttpClientOptions()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            DomainClient = await Intermediate.Main.CreateClient("DomainClient", new Intermediate.Models.HttpClientOptions()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            GlobalClient = await Intermediate.Main.CreateClient("GlobalClient", new Intermediate.Models.HttpClientOptions()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            Clients.Add(
            [
                EndpointClient,
                DomainClient,
                GlobalClient,
            ]);
        }

        [Test]
        public static async Task EndpointRateLimiting()
        {
            NamedClient? client = EndpointClient;
            if (client == null || client.Client == null)
            {
                Assert.That(client, Is.Not.Null, "Client is null");
                return;
            }

            string baseAddress = client.Client.BaseAddress?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                Assert.That(baseAddress, Is.Not.Null, "Client is null");
                return;
            }

            TimeSpan limit = TimeSpan.FromMilliseconds(100);
            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}/Get/RandomNumber"),
                    WholeDomain = false,
                },
            ];

            bool added = DR.Networking.RateLimiting.Add(urlRateLimits);
            if (!added)
            {
                Assert.Fail("Unable to add ratelimit");
                return;
            }

            (_, TimeSpan average, _) = await SendRateLimitRequest(client.Name, limit);
            if (average < limit)
            {
                Assert.Fail("The endpoint ratelimit duration average is shorter than expected");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.That(average, Is.GreaterThanOrEqualTo(limit), "The endpoint ratelimit duration average is shorter than expected");
        }

        [Test]
        public static async Task DomainRateLimiting()
        {
            NamedClient? client = DomainClient;
            if (client == null || client.Client == null)
            {
                Assert.That(client, Is.Not.Null, "Client is null");
                return;
            }

            string baseAddress = client.Client.BaseAddress?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                Assert.That(baseAddress, Is.Not.Null, "Client is null");
                return;
            }

            TimeSpan limit = TimeSpan.FromMilliseconds(115);
            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}"),
                    WholeDomain = true,
                },
            ];

            bool added = DR.Networking.RateLimiting.Add(urlRateLimits);
            if (!added)
            {
                Assert.Fail("Unable to add ratelimit");
                return;
            }

            List<string> requestUris =
            [
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            (_, TimeSpan average, _) = await SendRateLimitRequest(client.Name, limit, requestUris);
            if (average < limit)
            {
                Assert.Fail("The domain ratelimit duration average is shorter than expected");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.That(average, Is.GreaterThanOrEqualTo(limit), "The domain ratelimit duration average is shorter than expected");
        }

        [Test]
        public static async Task GlobalRateLimiting()
        {
            NamedClient? client = GlobalClient;
            if (client == null || client.Client == null)
            {
                Assert.That(client, Is.Not.Null, "Client is null");
                return;
            }

            string baseAddress = client.Client.BaseAddress?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                Assert.That(baseAddress, Is.Not.Null, "Client is null");
                return;
            }

            TimeSpan limit = TimeSpan.FromMilliseconds(130);
            DR.Networking.RateLimiting.UpdateGlobal(limit);

            (_, TimeSpan average, _) = await SendRateLimitRequest(client.Name, limit);
            if (average < limit)
            {
                Assert.Fail("The global ratelimit duration average is shorter than expected");
                return;
            }

            DR.Networking.RateLimiting.UpdateGlobal(null);
            Assert.That(average, Is.GreaterThanOrEqualTo(limit), "The global ratelimit duration average is shorter than expected");
        }

        private static async Task<(List<KeyValuePair<Result, TimeSpan>> Responses, TimeSpan Average, TimeSpan Shortest)> SendRateLimitRequest(string name, TimeSpan limit, List<string>? requestUris = null, int count = 15)
        {
            Stopwatch timer = new();
            List<KeyValuePair<Result, TimeSpan>> responses = [];

            for (int i = 0; i < count; i++)
            {
                if (i > 1)
                    timer.Start();

                Result response;
                if (requestUris != null)
                {
                    int index = RandomNumberGenerator.GetInt32(0, requestUris.Count);
                    response = await Request.Send(new(HttpMethod.Get, requestUris[index]), name);
                }
                else
                    response = await Request.Send(new(HttpMethod.Get, "Get/RandomNumber"), name);

                if (i > 1)
                {
                    responses.Add(new KeyValuePair<Result, TimeSpan>(response, timer.Elapsed));
                    timer.Reset();
                }
            }

            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            TimeSpan shortest = responses.Min(x => x.Value);

            return (responses, average, shortest);
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            if (EndpointClient != null)
                Clients.Remove(EndpointClient);

            if (DomainClient != null)
                Clients.Remove(DomainClient);

            if (GlobalClient != null)
                Clients.Remove(GlobalClient);
        }
    }
}