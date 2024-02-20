using DR.Networking.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    /// <summary>
    /// HTTP request methods.
    /// </summary>
    internal enum RequestTypes
    {
        Get,
        Head,
        Post,
        Put,
        Delete,
        Trace,
        Options,
        Connect,
        Patch
    }

    /// <summary>
    /// HTTP protocols.
    /// </summary>
    internal enum Protocol
    {
        Https,
        Http
    }

    internal class Main
    {
        /// <summary>
        /// The HttpClient used to make the requests.
        /// </summary>
        internal static HttpClient Client = new HttpClient(new StandardSocketsHttpHandler()
        {
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
            PooledConnectionLifetime = TimeSpan.FromMinutes(1),
        });

        /// <summary>
        /// The base function for making a network request to a specific url.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static async Task<ResultData> RequestBase<T>(string url, RequestTypes requestType, T body, T headers)
        {
            ResultData result = new ResultData();

            CheckUrlModel urlChecked = Base.CheckUrl(url);

            if (!urlChecked.Success)
            {
                return new ResultData()
                {
                    Success = urlChecked.Success,
                    Error = urlChecked.Error
                };
            }


            return result;
        }
    }
}
