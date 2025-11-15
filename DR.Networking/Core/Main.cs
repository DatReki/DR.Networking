using DR.Networking.Core.Attributes;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    /// <summary>
    /// HTTP request methods.
    /// </summary>
    internal enum RequestTypes
    {
        [Supported(true)]
        Get,
        [Supported(false)]
        Head,
        [Supported(true)]
        Post,
        [Supported(true)]
        Put,
        [Supported(true)]
        Delete,
        [Supported(false)]
        Trace,
        [Supported(false)]
        Options,
        [Supported(false)]
        Connect,
        [Supported(true)]
        Patch,
        [Supported(false)]
        Unknown
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
        /// A list of <see cref="RequestTypes"/> that are supported by the library.
        /// </summary>
        internal static List<RequestTypes> SupportedTypes { get; set; } = new List<RequestTypes>();

        /// <summary>
        /// The base function for making a network request to a specific url.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static async Task<ResultData> RequestBase<T>(HttpRequestMessage request, string? namedClient = null)
        {
            HttpClient client;
            if (string.IsNullOrWhiteSpace(namedClient))
                client = Settings.Client;
            else
                client = Settings.NamedClients.FirstOrDefault(x => x.Name == namedClient).Client ?? Settings.Client;

            string url = string.Empty;
            if (request.RequestUri != null)
                url = request.RequestUri.ToString();

            string baseAddress = string.Empty;
            if (client.BaseAddress != null)
                baseAddress = client.BaseAddress.ToString();

            string fullUrl = string.Empty;
            if (!string.IsNullOrWhiteSpace(baseAddress))
                fullUrl += baseAddress;

            if (!string.IsNullOrWhiteSpace(url))
                fullUrl += url;

            if (string.IsNullOrWhiteSpace(fullUrl))
            {
                return new ResultData()
                {
                    Success = false,
                    Url = string.Empty,
                    Error = "No url provided",
                    ErrorType = ErrorType.InvalidUrl,
                };
            }

            if (!request.Method.IsSupported(out RequestTypes requestType))
            {
                return new ResultData()
                {
                    Success = false,
                    Url = fullUrl,
                    Error = $"The selected HttpMethod '{request.Method}' is either not supported or not yet implemented",
                    ErrorType = ErrorType.HttpMethodNotSupported,
                };
            }

            CheckUrlModel urlChecked = await Base.CheckUrl(fullUrl);
            if (urlChecked.Success)
            {
                await RateLimiter.Check(fullUrl);
                HttpRequestMessage? clone = await request.Clone(fullUrl);
                return CreateResult(fullUrl, request, await client.SendAsync(request));
            }

            return new ResultData()
            {
                Success = urlChecked.Success,
                Url = GetResultUrl(urlChecked.Url, fullUrl),
                Error = urlChecked.Error,
                ErrorType = urlChecked.ErrorType,
            };
        }

        private static ResultData CreateResult(string url, HttpRequestMessage? request, HttpResponseMessage response)
        {
            ResultData result = new ResultData()
            {
                Success = true,
                Url = url,
                StatusCode = (int)response.StatusCode,
                Request = request,
                Response = response,
                Content = response.Content,
            };

            return result;
        }

        private static string GetResultUrl(Uri checkedUri, string url)
        {
            string checkedUrl = checkedUri.ToString();
            if (string.IsNullOrWhiteSpace(url))
                return checkedUrl;
            else if (string.IsNullOrWhiteSpace(checkedUrl))
                return url;
            else if (checkedUrl.ToLower().Trim() == "about:blank")
                return url;
            else
                return url;
        }
    }
}
