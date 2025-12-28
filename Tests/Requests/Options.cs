using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    public class Options
    {
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
    }
}
