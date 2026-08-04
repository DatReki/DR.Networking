using Backend.Core;
using DR.Networking;
using DR.Networking.Models;
using System.Net;

namespace Tests.Failures
{
    [TestFixture]
    internal class RateLimiting
    {
        private static NamedClient? EndpointClient { get; set; } = null;
        private static NamedClient? DomainClient { get; set; } = null;
        private static NamedClient? GlobalClient { get; set; } = null;
        private static List<UrlRateLimit> RateLimits { get; set; } = [];

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
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

            RateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = TimeSpan.FromMilliseconds(100),
                    Uri = DomainClient.Client?.BaseAddress ?? new Uri("about:blank"),
                    WholeDomain = true,
                },
            ];

            DR.Networking.RateLimiting.Add(RateLimits);
        }

        [Test]
        public static async Task RateLimitTimeout()
        {
            NamedClient? client = DomainClient;
            if (client == null || client.Client == null)
            {
                Assert.That(client, Is.Not.Null, "Client is null");
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

            DR.Networking.RateLimiting.UpdateRateLimitTimeout(TimeSpan.FromMilliseconds(50));
            List<Result> result = await Core.MultipleRequests.SendParallelRequests(client.Name, requestUris);
            DR.Networking.RateLimiting.UpdateRateLimitTimeout(null);

            if (result.Any(x => x.ErrorType == ErrorType.RateLimitTimeout))
                Assert.Pass();
            else
                Assert.Fail($"Did not recieve any results containing '{ErrorType.RateLimitTimeout}'");
        }

        [Test]
        public static async Task RateLimitController()
        {
            NamedClient? client = EndpointClient;
            if (client == null || client.Client == null)
            {
                Assert.That(client, Is.Not.Null, "Client is null");
                return;
            }

            string baseAddress = client.GetBaseAddress();
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                Assert.That(baseAddress, Is.Not.Null, "Client is null");
                return;
            }

            TimeSpan limit = TimeSpan.FromMilliseconds(95);
            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = limit,
                    Uri = new Uri($"{baseAddress}/Ratelimit/Basic"),
                    WholeDomain = false,
                },
            ];

            string error = Core.RateLimitChecks.RemoveExistingRateLimits(urlRateLimits);
            if (!string.IsNullOrEmpty(error))
            {
                Assert.Fail(error);
                return;

            }

            bool added = DR.Networking.RateLimiting.Add(urlRateLimits);
            if (!added)
            {
                Assert.Fail("Unable to add ratelimit");
                return;
            }

            List<string> requestUris =
            [
                "Ratelimit/Basic",
            ];

            List<Result> result = await Core.MultipleRequests.SendParallelRequests(client.Name, requestUris);
            Assert.That(result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests), Is.True, $"Status codes did not contain any '{HttpStatusCode.TooManyRequests}'");
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.Remove(RateLimits);

            if (EndpointClient != null)
                Clients.Remove(EndpointClient);

            if (DomainClient != null)
                Clients.Remove(DomainClient);

            if (GlobalClient != null)
                Clients.Remove(GlobalClient);
        }
    }
}
