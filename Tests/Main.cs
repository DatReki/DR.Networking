using DR.Networking;
using DR.Networking.Models;
using Generate.Models;
using Intermediate.Models;

namespace Tests
{
    [SetUpFixture]
    public class Main
    {
        internal static Dictionary<string, string> ApiUsers { get; set; } = [];
        internal static Configuration? Configuration { get; set; }

        internal static readonly bool UseHttps = true;

        internal static readonly TimeSpan RateLimitDuration = TimeSpan.FromMilliseconds(101);

        [OneTimeSetUp]
        public async Task Setup()
        {
            HttpClientOptions options = new()
            {
#if DEBUG
                Timeout = TimeSpan.FromMinutes(10),
#else
                Timeout = TimeSpan.FromSeconds(5),
#endif
            };

            NamedClient ipClient = await Intermediate.Main.CreateClient("ip-testing", options);
            ipClient.Client?.BaseAddress = null;

            List<NamedClient> clients =
            [
                await Intermediate.Main.CreateClient("local-testing", options),
                ipClient
            ];

            Configuration = new Configuration(new ConfigurationOptions()
            {
                NamedClients = clients,
                UseHttpsByDefault = UseHttps,
                CloneRequestMessage = true,
                ValidateUrl = true,
            });

            ApiUser apiUser = Intermediate.Generate.ApiUser();
            ApiUsers.Add(apiUser.ClientId.ToString(), apiUser.ClientSecret);
        }
    }
}