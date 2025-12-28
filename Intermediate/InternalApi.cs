using Intermediate.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.NetworkInformation;

namespace Intermediate
{
    public class InternalApi
    {
        /// <summary>
        /// A list of the ports we're already using.
        /// </summary>
        public static List<int> UsedPorts { get; private set; } = [];

        /// <summary>
        /// Create a new HTTP client which utilizes the <see cref="Api"/> project.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<HttpClient> GetClient(HttpClientOptions options)
        {
            int port = GetPort();
            WebApplicationFactory<Api.Program> api = new();
            api.WithWebHostBuilder(x =>
            {
                x.UseUrls($"http://*:{port}/");
            });

            HttpClient client = api.CreateClient();
            if (options.DefaultHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in options.DefaultHeaders)
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            if (options.Version != null)
                client.DefaultRequestVersion = options.Version;

            if (options.Policy != null)
                client.DefaultVersionPolicy = (HttpVersionPolicy)options.Policy;

            if (options.MaxBufferSize != null)
                client.MaxResponseContentBufferSize = (long)options.MaxBufferSize;

            if (options.Timeout != null)
                client.Timeout = (TimeSpan)options.Timeout;

            return client;
        }

        /// <summary>
        /// Find a port which is currently not in use.
        /// </summary>
        /// <returns></returns>
        private static int GetPort()
        {
            int port = 6000;
            if (UsedPorts.Count != 0)
            {
                int max = UsedPorts.Max();
                if (max >= 65500)
                    UsedPorts.Clear();
                else
                    port = UsedPorts.Max() + 1;
            }

            bool portFound = false;
            while (!portFound)
            {
                try
                {
                    IPGlobalProperties ipProps = IPGlobalProperties.GetIPGlobalProperties();
                    IPEndPoint[] endpoints = ipProps.GetActiveTcpListeners();

                    if (endpoints.Any(x => x.Port != port))
                        portFound = true;
                }
                catch
                {
                    port++;
                }
            }

            UsedPorts.Add(port);
            return port;
        }
    }
}
