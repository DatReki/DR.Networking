using DR.Networking;
using DR.Networking.Models;

namespace Tests.Failures
{
    [TestFixture]
    public class General
    {
        [Test]
        public async Task EmptyUrl()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "");
            Result response = await Request.Send(request);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.Not.True);
                Assert.That(response.ErrorType, Is.EqualTo(ErrorType.InvalidUrl), $"Returned error type '{response.ErrorType}' while '{ErrorType.InvalidUrl}' was expected.");
            }
        }

        [Test]
        public async Task MethodNotSupported()
        {

            HttpRequestMessage request = new(new HttpMethod("Testing"), "Get");
            Result response = await Request.Send(request, Clients.GetClientNames().First());

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.Not.True);
                Assert.That(response.ErrorType, Is.EqualTo(ErrorType.HttpMethodNotSupported), $"Returned error type '{response.ErrorType}' while '{ErrorType.HttpMethodNotSupported}' was expected.");
            }
        }

        [Test]
        public async Task InvalidUrl()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Testing éü.com");
            Result response = await Request.Send(request);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.Not.True);
                Assert.That(response.ErrorType, Is.EqualTo(ErrorType.InvalidUrl), $"Returned error type '{response.ErrorType}' while '{ErrorType.InvalidUrl}' was expected.");
            }
        }

        [Test]
        public async Task InvalidDomain()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Testing");
            Result response = await Request.Send(request);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response.Success, Is.Not.True);
                Assert.That(response.ErrorType, Is.EqualTo(ErrorType.InvalidDomain), $"Returned error type '{response.ErrorType}' while '{ErrorType.InvalidDomain}' was expected.");
            }
        }        
    }
}
