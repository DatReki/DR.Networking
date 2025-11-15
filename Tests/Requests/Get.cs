using DR.Networking;
using DR.Networking.Models;

namespace Tests.Requests
{
    [TestFixture]
    public class Get
    {
        [Test]
        public async Task NamedClient()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Get");
            ResultData response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response.Success, Is.True, "Unable to send GET request with a named client");

                string content = string.Empty;
                if (response.Content != null)
                    content = await response.Content.ReadAsStringAsync();

                Assert.That(content, Is.EqualTo("This is the result of the example GET request"), "GET request with named client did not receive excpected reply");
            }
        }
    }
}
