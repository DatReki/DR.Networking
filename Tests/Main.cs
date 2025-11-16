using Api.Models;
using DR.Networking;
using DR.Networking.Models;

namespace Tests
{
    [SetUpFixture]
    public class Main
    {
        internal static Dictionary<string, string> ApiUsers { get; set; } = [];
        internal static Configuration? Configuration { get; set; }

        [OneTimeSetUp]
        public async Task Setup()
        {
            List<NamedClient> clients =
            [
                await Backend.Main.CreateClient("testing", new Backend.Models.HttpClientOptions(TimeSpan.FromSeconds(5)))
            ];
            Configuration = new Configuration(namedClients: clients);

            ApiUser apiUser = Backend.Generate.ApiUser();
            ApiUsers.Add(apiUser.ClientId.ToString(), apiUser.ClientSecret);
        }
    }
}