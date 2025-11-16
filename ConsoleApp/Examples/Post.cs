using Api.Models;
using DR.Networking;
using DR.Networking.Models;
using Newtonsoft.Json;
using System.Text;

namespace ConsoleApp.Examples
{
    internal class Post
    {
        internal static async Task AddUser()
        {
            User user = Backend.Generate.User();
            NamedClient client = await Backend.Main.CreateClient("test", new Backend.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));

            List<NamedClient> namedClients =
            [
                client
            ];

            _ = new Configuration(namedClients: namedClients);
            HttpRequestMessage request = new(HttpMethod.Post, "Post/CreateUser")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            ResultData response = await Request.Send(request, client.Name);
            await Program.ShowResult(response);
        }
    }
}
