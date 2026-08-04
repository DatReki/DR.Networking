using Backend;
using DR.Networking;
using DR.Networking.Models;
using Intermediate;
using System.Net;
using System.Net.Sockets;

namespace Tests.Requests
{
    [TestFixture]
    public class Get
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        [NonParallelizable]
        public async Task NamedClient()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Get");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }

        [Test]
        [NonParallelizable]
        public async Task ValidIpv4()
        {
            HttpRequestMessage request = new(HttpMethod.Get, $"127.0.0.1:{InternalApi.UsedPorts.Last()}/Get");
            Result response = await Request.Send(request, "ip-testing");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }

        [Test]
        [NonParallelizable]
        public async Task ValidUrlIpv4()
        {
            // Check if you provide a valid RequestUri & BaseAddress if the library will use just the RequestUri instead of adding the BaseAddress & RequestUri together.
            // Because if it adds the BaseAddress & RequestUri together this will create a url that doesn't exist on the API.
            HttpRequestMessage request = new(HttpMethod.Get, $"https://127.0.0.1:{InternalApi.UsedPorts.Last()}/Get");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }

        [Test]
        [NonParallelizable]
        public async Task ValidIpv6()
        {
            if (!Tools.Ipv6Available())
            {
                Assert.Ignore("IPv6 is not available");
                return;
            }

            HttpRequestMessage request = new(HttpMethod.Get, $"[0:0:0:0:0:0:0:1]:{InternalApi.UsedPorts.Last()}/Get");
            Result response = await Request.Send(request, "ip-testing");

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }

        [Test]
        [NonParallelizable]
        public async Task ValidUrlIpv6()
        {
            if (!Tools.Ipv6Available())
            {
                Assert.Ignore("IPv6 is not available");
                return;
            }

            // Check if you provide a valid RequestUri & BaseAddress if the library will use just the RequestUri instead of adding the BaseAddress & RequestUri together.
            // Because if it adds the BaseAddress & RequestUri together this will create a url that doesn't exist on the API.
            HttpRequestMessage request = new(HttpMethod.Get, $"https://[0:0:0:0:0:0:0:1]:{InternalApi.UsedPorts.Last()}/Get");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }

        [Test]
        [NonParallelizable]
        public async Task CompareIpv4()
        {
            IPAddress? address = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            if (address == null)
            {
                Assert.Fail("Could not get IPv4 address");
                return;
            }

            HttpRequestMessage request = new(HttpMethod.Get, address.ToString());
            Result response = await Request.Send(request, "ip-testing");
            string compare = Main.UseHttps ? $"https://{address}/" : $"http://{address}/";

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Url, Is.EqualTo(compare));
            }
        }

        [Test]
        [NonParallelizable]
        public async Task CompareIpv6()
        {
            if (!Tools.TryGetIpv6(out IPAddress? address) || address == null)
            {
                Assert.Ignore("Either IPv6 is not available or could not get IPv6 address");
                return;
            }

            HttpRequestMessage request = new(HttpMethod.Get, address.ToString());
            Result response = await Request.Send(request, "ip-testing");

            string compare = string.Empty;
            if (Uri.TryCreate(Main.UseHttps ? $"https://[{address}]/" : $"http://[{address}]/", UriKind.Absolute, out Uri? tmp) && tmp != null)
                compare = tmp.ToString();

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(string.IsNullOrWhiteSpace(compare), Is.False);
                Assert.That(response.Url, Is.EqualTo(compare));
            }
        }


        [Test]
        public static async Task ParallelRequests()
        {
            List<string> requestUris =
            [
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            List<Result> result = await Core.MultipleRequests.SendParallelRequests(Clients.GetClientNames().First(), requestUris);
            if (result.Any(x => x.StatusCode == (int)HttpStatusCode.TooManyRequests))
                Assert.Fail($"One or more requests returned '{HttpStatusCode.TooManyRequests}'");
            else if (result.Any(x => !x.Success))
                Assert.Fail($"One or more parallel '{HttpMethod.Get}' requests failed");
            else
                Assert.Pass();
        }

        [Test]
        [NonParallelizable]
        public static async Task LoopedRequests()
        {
            List<string> requestUris =
            [
                "Get/RandomNumber",
                "Get/RandomString",
                "Get/RandomText",
                "Get/RandomJson",
                "Get/RandomXml",
            ];

            List<Result> result = await Core.MultipleRequests.SendLoopedRequest(Clients.GetClientNames().First(), requestUris);
            if (result.Any(x => !x.Success))
                Assert.Fail($"One or more looped '{HttpMethod.Get}' requests failed");
            else
                Assert.Pass();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
