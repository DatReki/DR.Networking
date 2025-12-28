using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    public class Head
    {
        [Test]
        public async Task HeadExample()
        {
            HttpRequestMessage request = new(HttpMethod.Head, @$"Head/Example");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null, "HEAD request is empty");
                Assert.That(response.Success, Is.True, "Unable to send HEAD request");
            }
        }
    }
}
