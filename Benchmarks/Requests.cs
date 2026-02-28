using BenchmarkDotNet.Attributes;
using DR.Networking;
using DR.Networking.Models;
using Intermediate.Models;
using StandardClient = System.Net.Http.HttpClient;

namespace Benchmarks
{
    public class Requests
    {
        internal static bool LibraryConfigured { get; set; } = false;
        internal static StandardClient Client { get; set; } = new();
        internal static ConfigurationOptions Options { get; set; } = new();
        internal static readonly bool UseHttps = true;

        internal static async Task Setup()
        {
            string clientName = "benchmarking";
            TimeSpan timeout = TimeSpan.FromSeconds(5);

            NamedClient? namedClient = Clients.GetClient(clientName);
            if (namedClient == null)
            {
                HttpClientOptions options = new()
                {
                    Timeout = timeout,
                };

                namedClient = await Intermediate.Main.CreateClient(clientName, options);
                bool added = Clients.Add(namedClient);
                if (!added)
                    throw new Exception("Failed to add the named client to the library.");
            }

            if (!LibraryConfigured)
            {
                Options = new ConfigurationOptions()
                {
                    UseHttpsByDefault = UseHttps,
                    CloneRequestMessage = true,
                    ValidateUrl = true,
                };

                _ = new Configuration(Options);
                LibraryConfigured = true;
            }

            if (namedClient != null)
            {
                if (Client != namedClient.Client)
                    Client = namedClient.Client ?? new StandardClient();
            }
            else
                throw new Exception("Could not get the HttpClient from the NamedClient.");
        }

#pragma warning disable CA1822 // Mark members as static
        [GlobalSetup]
        public async Task GlobalSetup()
            => await Setup();

        /// <summary>
        /// Default get request with the <see cref="System.Net.Http.HttpClient"/>
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<string> HttpClient()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Get");
            HttpResponseMessage response = await Client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Default get request with the DR.Networking library.
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<string> Networking()
        {
            HttpRequestMessage request = new(HttpMethod.Get, "Get");
            Result result = await Request.Send(request, Clients.GetClientNames().First());
            return await (result.Content?.ReadAsStringAsync() ?? Task.FromResult(string.Empty));
        }
#pragma warning restore CA1822 // Mark members as static
    }
}
