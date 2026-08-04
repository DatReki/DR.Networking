using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    [NonParallelizable]
    public class Connect
    {
        [Test]
        public async Task ConnectExample()
        {
            HttpRequestMessage request = new(HttpMethod.Connect, @$"Connect/Example");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null, "CONNECT request is empty");
                Assert.That(response.Success, Is.True, "Unable to send CONNECT request");
            }
        }
    }
}
