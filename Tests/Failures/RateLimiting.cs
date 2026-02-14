using DR.Networking;
using DR.Networking.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;

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
            (List<KeyValuePair<Result, TimeSpan>> responses, _, _) = await SendParallelRateLimitRequest(client.Name, TimeSpan.FromMilliseconds(100), requestUris);
            DR.Networking.RateLimiting.UpdateRateLimitTimeout(null);

            if (responses.Any(x => x.Key.ErrorType == ErrorType.RateLimitTimeout))
                Assert.Pass();
            else
                Assert.Fail("Not all responses are rate limit timeouts");
        }

        private static async Task<(List<KeyValuePair<Result, TimeSpan>> Responses, TimeSpan Average, TimeSpan Shortest)> SendParallelRateLimitRequest(string name, TimeSpan limit, List<string>? requestUris = null, int count = 15)
        {
            ConcurrentBag<KeyValuePair<Result, TimeSpan>> responses = [];
            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = 2,
            };

            await Parallel.ForEachAsync(Enumerable.Repeat(string.Empty, count), options, async (item, token) =>
            {
                Result response;
                Stopwatch timer = Stopwatch.StartNew();

                if (requestUris != null)
                {
                    int index = RandomNumberGenerator.GetInt32(0, requestUris.Count);
                    response = await Request.Send(new(HttpMethod.Get, requestUris[index]), name);
                }
                else
                    response = await Request.Send(new(HttpMethod.Get, "Get/RandomNumber"), name);

                TimeSpan elapsed = timer.Elapsed;
                responses.Add(new KeyValuePair<Result, TimeSpan>(response, elapsed));
                timer.Reset();
            });

            IEnumerable<TimeSpan> tooShort = responses.Where(x => x.Value < limit).Select(x => x.Value);
            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            TimeSpan shortest = tooShort.First(x => x.TotalMilliseconds == tooShort.Min(y => y.TotalMilliseconds));

            return (responses.ToList(), average, shortest);
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
