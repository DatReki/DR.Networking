using DR.Networking;
using DR.Networking.Models;
using Intermediate.Models;

namespace Intermediate
{
    public class Main
    {
        /// <summary>
        /// Create a named client which will have the <see cref="HttpClient.BaseAddress"/> set to the URL of the <see cref="Api"/> project.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<NamedClient> CreateClient(string name, HttpClientOptions options)
        {
            HttpClient httpClient = await InternalApi.GetClient(options);
            NamedClient namedClient = Clients.CreateClient(name, httpClient);

            return namedClient;
        }


        /// <summary>
        /// Create a named client which will have the <see cref="HttpClient.BaseAddress"/> set to the URL of the <see cref="Api"/> project.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<NamedClient> CreateClient<T>(HttpClientOptions options)
        {
            HttpClient httpClient = await InternalApi.GetClient(options);
            NamedClient namedClient = Clients.CreateClient<T>(httpClient);

            return namedClient;
        }
    }
}
