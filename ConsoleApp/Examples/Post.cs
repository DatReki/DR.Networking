using DR.Networking;
using DR.Networking.Models;
using Generate.Models;
using Intermediate;
using Newtonsoft.Json;
using System.Text;

namespace ConsoleApp.Examples
{
    internal class Post
    {
        internal static async Task AddUser()
        {
            User user = Intermediate.Generate.User();
            NamedClient client = await Main.CreateClient("test", new Intermediate.Models.HttpClientOptions(TimeSpan.FromSeconds(5)));

            List<NamedClient> namedClients =
            [
                client
            ];

            _ = new Configuration(new ConfigurationOptions()
            {
                NamedClients = namedClients,
                CloneRequestMessage = true,
            });

            HttpRequestMessage request = new(HttpMethod.Post, "Post/CreateUser")
            {
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            Result response = await Request.Send(request, client.Name);
            await Program.ShowResult(response);
        }
    }
}
