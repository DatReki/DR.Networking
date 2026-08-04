using Backend.Core;
using DR.Networking;
using DR.Networking.Models;
using System.Net;

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
        [NonParallelizable]
        public static async Task EndpointRateLimiting()
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

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = Main.RateLimitDuration,
                    Uri = new Uri($"{baseAddress}/Get/RandomNumber"),
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

            List<Result> result = await Core.MultipleRequests.SendLoopedRequest(client.Name);
            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
            {
                Assert.Fail($"One or more requests either returned '{HttpStatusCode.TooManyRequests}'");
                return;
            }
            else if (result.Any(x => x.StatusCode != (int)HttpStatusCode.OK))
            {
                Assert.Fail($"One or more requests didn't return '{HttpStatusCode.OK}'");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.Pass();
        }

        [Test]
        [NonParallelizable]
        public static async Task DomainRateLimiting()
        {
            NamedClient? client = DomainClient;
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

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = Main.RateLimitDuration,
                    Uri = new Uri($"{baseAddress}"),
                    WholeDomain = true,
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
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            List<Result> result = await Core.MultipleRequests.SendLoopedRequest(client.Name, requestUris);
            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
            {
                Assert.Fail($"One or more requests either returned '{HttpStatusCode.TooManyRequests}'");
                return;
            }
            else if (result.Any(x => x.StatusCode != (int)HttpStatusCode.OK))
            {
                Assert.Fail($"One or more requests didn't return '{HttpStatusCode.OK}'");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.Pass();
        }

        [Test]
        public static async Task DomainParallelRateLimiting()
        {
            NamedClient? client = DomainClient;
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

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = Main.RateLimitDuration,
                    Uri = new Uri($"{baseAddress}"),
                    WholeDomain = true,
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
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            List<Result> result = await Core.MultipleRequests.SendParallelRequests(client.Name, requestUris);
            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
            {
                Assert.Fail($"One or more requests returned '{HttpStatusCode.TooManyRequests}'");
                return;
            }
            else if (result.Any(x => x.StatusCode != (int)HttpStatusCode.OK))
            {
                Assert.Fail($"One or more requests didn't return '{HttpStatusCode.OK}'");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.Pass();
        }

        [Test]
        [NonParallelizable]
        public static async Task GlobalRateLimiting()
        {
            NamedClient? client = GlobalClient;
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

            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
            List<Result> result = await Core.MultipleRequests.SendLoopedRequest(client.Name);

            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
            {
                Assert.Fail($"One or more requests either returned '{HttpStatusCode.TooManyRequests}'");
                return;
            }
            else if (result.Any(x => x.StatusCode != (int)HttpStatusCode.OK))
            {
                Assert.Fail($"One or more requests didn't return '{HttpStatusCode.OK}'");
                return;
            }

            DR.Networking.RateLimiting.UpdateGlobal(null);
            Assert.Pass();
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

            List<UrlRateLimit> urlRateLimits =
            [
                new UrlRateLimit()
                {
                    Duration = Main.RateLimitDuration,
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

            List<Result> result = await Core.MultipleRequests.SendParallelRequests(client.Name, requestUris, 150);
            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
            {
                Assert.Fail($"One or more requests either returned '{HttpStatusCode.TooManyRequests}'");
                return;
            }
            else if (result.Any(x => x.StatusCode != (int)HttpStatusCode.OK))
            {
                Assert.Fail($"One or more requests didn't return '{HttpStatusCode.OK}'");
                return;
            }

            bool removed = DR.Networking.RateLimiting.Remove(urlRateLimits);
            if (!removed)
            {
                Assert.Fail("Unable to remove ratelimit");
                return;
            }

            Assert.Pass();
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