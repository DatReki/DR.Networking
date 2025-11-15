using DR.Networking.Core;
using DR.Networking.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace DR.Networking
{
    public class Request
    {
        /// <summary>
        /// Send a HTTP request without a named client.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ResultData> Send(HttpRequestMessage request)
            => await Main.RequestBase<string?>(request, string.Empty);

        /// <summary>
        /// Send a HTTP request with a named client.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="clientName"></param>
        /// <returns></returns>
        public static async Task<ResultData> Send(HttpRequestMessage request, string clientName)
            => await Main.RequestBase<string?>(request, clientName);

        /// <summary>
        /// Send a HTTP request with a named client.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<ResultData> Send<T>(HttpRequestMessage request)
            => await Main.RequestBase<string?>(request, typeof(T).GetClientName());
    }
}
