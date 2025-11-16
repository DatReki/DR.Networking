using Api.Models;
using DR.Networking;
using DR.Networking.Models;
using Newtonsoft.Json;
using System.Text;

namespace Tests.Requests
{
    [TestFixture]
    public class Post
    {
        [Test]
        public async Task CheckUsers()
        {
            User user = Backend.Generate.User();
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

            HttpRequestMessage checkRequest = new(HttpMethod.Get, "Get/GetUsers");
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

            List<User> users = [];
            try
            {
                List<User>? temp = JsonConvert.DeserializeObject<List<User>>(json);
                if (temp == null)
                {
                    Assert.Fail("Unable to deserialize users list");
                    return;
                }
                else
                    users = temp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Assert.Fail("An exception occured while trying to deserialize users list");         
                return;
            }

            bool userFound = users.Any(x => x.Username == user.Username && x.Password == user.Password && x.Email == user.Email);
            Assert.That(userFound, Is.True, "Unable to find the created user");
        }
    }
}
