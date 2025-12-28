using Api.Models;
using DR.Networking;
using DR.Networking.Models;
using Generate.Models;
using Newtonsoft.Json;
using System.Text;

namespace Tests.Core
{
    public class LibraryRequests
    {
        public static async Task<(List<User>? Users, string Error)> GetUsers()
        {
            List<User>? result = null;

            HttpRequestMessage checkRequest = new(HttpMethod.Get, @$"Get/GetUsers");
            Result getResponse = await Request.Send(checkRequest, Clients.GetClientNames().First());
            if (getResponse == null || !getResponse.Success)
                return (result, "Unable to get existing users");

            if (getResponse.Content == null)
                return (result, "Unable read gets users response");

            string json = await getResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return (result, "Get user response is empty");

            try
            {
                result = JsonConvert.DeserializeObject<List<User>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (result, "An exception occured while trying to deserialize the users");
            }

            if (result == null)
                return (result, $"Get users returned null");

            return (result, string.Empty);
        }

        public static async Task<(User? User, string Error)> GetUserByEmail(string email)
        {
            User? result = null;

            HttpRequestMessage checkRequest = new(HttpMethod.Get, @$"Get/GetUserByEmail?email={email}");
            Result checkResponse = await Request.Send(checkRequest, Clients.GetClientNames().First());
            if (checkResponse == null || !checkResponse.Success)
                return (result, "Unable to get user");

            if (checkResponse.Content == null)
                return (result, "Unable read get user response");

            string json = await checkResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return (result, "Get user response is empty");

            try
            {
                result = JsonConvert.DeserializeObject<User>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (result, "An exception occured while trying to deserialize the user");
            }

            return (result, string.Empty);
        }

        public static async Task<(AuthorizedResponse<User>? User, string Error)> GetUserAuthorized(string email, KeyValuePair<string, string> authUser)
        {
            AuthorizedResponse<User>? result = null;
            HttpRequestMessage checkRequest = new(HttpMethod.Get, @$"Get/GetUserAuthorized?email={email}");
            checkRequest.Headers.Add("X-Request-ID", authUser.Key);
            checkRequest.Headers.Add("Authorization", authUser.Value);

            Result checkResponse = await Request.Send(checkRequest, Clients.GetClientNames().First());
            if (checkResponse == null || !checkResponse.Success)
                return (result, "Unable to get user");

            if (checkResponse.Content == null)
                return (result, "Unable read get user response");

            string json = await checkResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return (result, "Get user response is empty");

            try
            {
                result = JsonConvert.DeserializeObject<AuthorizedResponse<User>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (result, "An exception occured while trying to deserialize the user");
            }

            return (result, string.Empty);
        }

        public static async Task<string> CreateUser(User user)
        {
            HttpRequestMessage createRequest = new(HttpMethod.Post, "Post/CreateUser")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            Result createResponse = await Request.Send(createRequest, Clients.GetClientNames().First());
            if (createResponse == null || !createResponse.Success)
                return "Unable to create user";

            return string.Empty;
        }

        public static async Task<(string Response, string Error)> EditUser(string email, User user)
        {
            HttpRequestMessage createRequest = new(HttpMethod.Patch, $"Patch/EditUser?email={email}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            Result editResponse = await Request.Send(createRequest, Clients.GetClientNames().First());
            if (editResponse == null || !editResponse.Success)
                return (string.Empty, "Unable to get changed user");

            if (editResponse.Content == null)
                return (string.Empty, "Unable read changed user response");

            return (await editResponse.Content.ReadAsStringAsync(), string.Empty);
        }
    }
}
