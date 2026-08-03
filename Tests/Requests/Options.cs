using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    public class Options
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DR.Networking.RateLimiting.UpdateGlobal(Main.RateLimitDuration);
        }

        [Test]
        public async Task OptionsExample()
        {
            HttpRequestMessage request = new(HttpMethod.Options, @$"Options/Example");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null, "OPTIONS request is empty");
                Assert.That(response.Success, Is.True, "Unable to send OPTIONS request");
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            DR.Networking.RateLimiting.UpdateGlobal(null);
        }
    }
}
