using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    [NonParallelizable]
    public class Trace
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        public async Task TraceExample()
        {
            HttpRequestMessage request = new(HttpMethod.Trace, @$"Trace/Example");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null, "TRACE request is empty");
                Assert.That(response.Success, Is.True, "Unable to send TRACE request");
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
