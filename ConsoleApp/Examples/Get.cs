using ConsoleApp.Models.Get;
using DR.Networking;
using DR.Networking.Models;

namespace ConsoleApp.Examples
{
    internal class Get
    {
        internal static async Task NamedClientString()
        {
            var client = Clients.CreateClient("test", new HttpClient()
            {
                BaseAddress = new Uri("https://github.com/DatReki/"),
                Timeout = TimeSpan.FromSeconds(5)
            });

            Program.ShowResult(await ExampleCall("DR.Cache", client));
        }

        internal static async Task NamedClientType()
        {
            var client = Clients.CreateClient<ExampleClass>(new HttpClient()
            {
                BaseAddress = new Uri("https://github.com/DatReki/"),
                Timeout = TimeSpan.FromSeconds(5)
            });

            Program.ShowResult(await ExampleCall("DR.Cache", client));
        }

        internal static async Task InavlidUrl()
        {
            var client = Clients.CreateClient<ExampleClass>(new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            Program.ShowResult(await ExampleCall("12.4121.134", client));
        }

        internal static async Task NoUrl()
        {
            var client = Clients.CreateClient<ExampleClass>(new HttpClient()
            {
                Timeout = TimeSpan.FromSeconds(5)
            });

            Program.ShowResult(await ExampleCall(string.Empty, client));
        }


        private static async Task<ResultData> ExampleCall(string url, NamedClient client)
        {
            List<NamedClient> namedClients =
            [
                client
            ];

            _ = new Configuration(namedClients: namedClients);
            HttpRequestMessage request = new(HttpMethod.Get, url);

            return await Request.Send(request, client.Name);
        }
    }
}
