using Api.Models;
using Backend;
using DR.Networking;
using DR.Networking.Models;
using Newtonsoft.Json;
using System.Text;
using System.Web;

namespace Tests.Requests
{
    [TestFixture]
    public class Post
    {
        [Test]
        public async Task CheckUser()
        {
            User user = Generate.User();
            HttpRequestMessage createRequest = new(HttpMethod.Post, "Post/CreateUser")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            ResultData createResponse = await Request.Send(createRequest, Clients.GetClientNames().First());
            if (createResponse == null || !createResponse.Success)
            {
                Assert.Fail("Unable to create user");
                return;
            }

            string encodedEmail = HttpUtility.UrlEncode(user.Email);
            HttpRequestMessage checkRequest = new(HttpMethod.Get, @$"Get/GetUserByEmail?email={encodedEmail}");
            ResultData checkResponse = await Request.Send(checkRequest, Clients.GetClientNames().First());
            if (checkResponse == null || !checkResponse.Success)
            {
                Assert.Fail("Unable to get all users");
                return;
            }

            if (checkResponse.Content == null)
            {
                Assert.Fail("Unable read get all users response");
                return;
            }

            string json = await checkResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                Assert.Fail("Get all users response is empty");
                return;
            }

            User? foundUser;
            try
            {
                foundUser = JsonConvert.DeserializeObject<User>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Fail("An exception occured while trying to deserialize the user");
                return;
            }

            Assert.That(foundUser.Compare(user), Is.True, "Unable to find the created user");
        }

        [Test]
        public async Task CheckUserAuthorized()
        {
            User user = Generate.User();
            HttpRequestMessage createRequest = new(HttpMethod.Post, "Post/CreateUser")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            ResultData createResponse = await Request.Send(createRequest, Clients.GetClientNames().First());
            if (createResponse == null || !createResponse.Success)
            {
                Assert.Fail("Unable to create user");
                return;
            }

            string encodedEmail = HttpUtility.UrlEncode(user.Email);
            HttpRequestMessage checkRequest = new(HttpMethod.Get, @$"Get/GetUserAuthorized?email={encodedEmail}");

            var apiUser = Main.ApiUsers.First();
            checkRequest.Headers.Add("X-Request-ID", apiUser.Key);
            checkRequest.Headers.Add("Authorization", apiUser.Value);

            ResultData checkResponse = await Request.Send(checkRequest, Clients.GetClientNames().First());
            if (checkResponse == null || !checkResponse.Success)
            {
                Assert.Fail("Unable to get all users");
                return;
            }

            if (checkResponse.Content == null)
            {
                Assert.Fail("Unable read get all users response");
                return;
            }

            string json = await checkResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                Assert.Fail("Get all users response is empty");
                return;
            }

            AuthorizedResponse<User>? foundUser;
            try
            {
                foundUser = JsonConvert.DeserializeObject<AuthorizedResponse<User>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Fail("An exception occured while trying to deserialize the user");
                return;
            }

            Assert.That(foundUser.Compare(user), Is.True, "Unable to find the created user");
        }
    }
}
