using DR.Networking.Models;
using System;
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
        /// The base function for making a network request to a specific url.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="body"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal static async Task<ResultData> RequestBase<T>(string url, RequestTypes requestType, T body, T headers, string? namedClient = null)
        {
            string baseAddress = string.Empty;
            HttpClient client = Settings.NamedClients.FirstOrDefault(x => x.Name == namedClient).Client ?? Settings.Client;
            
            if (client.BaseAddress != null)
                baseAddress = client.BaseAddress.ToString();

            if (string.IsNullOrWhiteSpace(baseAddress) && string.IsNullOrWhiteSpace(url))
            {
                return new ResultData()
                {
                    Success = false,
                    Url = string.Empty,
                    Error = "No url provided",
                    ErrorType = ErrorType.InvalidUrl,
                };
            }

            string fullUrl = url;
            if (!string.IsNullOrWhiteSpace(baseAddress))
                fullUrl = baseAddress + url;

            CheckUrlModel urlChecked = await Base.CheckUrl(fullUrl);
            if (urlChecked.Success)
            {
                await RateLimiter.Check(fullUrl);

                switch (requestType)
                {
                    case RequestTypes.Head:
                        break;
                    case RequestTypes.Post:
                        break;
                    case RequestTypes.Put:
                        break;
                    case RequestTypes.Delete:
                        break;
                    case RequestTypes.Trace:
                        break;
                    case RequestTypes.Options:
                        break;
                    case RequestTypes.Connect:
                        break;
                    case RequestTypes.Patch:
                        break;
                    case RequestTypes.Get:
                        return CreateResult(fullUrl, await client.GetAsync(url));                        
                }

                return new ResultData()
                {
                    Success = false,
                    Url = GetResultUrl(urlChecked.Url, fullUrl),
                    Error = $"The selected request type: '{Enum.GetName(typeof(RequestTypes), requestType)}' is either not supported or not yet implemented",
                    ErrorType = ErrorType.RequestTypeNotSupported,
                };
            }

            return new ResultData()
            {
                Success = urlChecked.Success,
                Url = GetResultUrl(urlChecked.Url, fullUrl),
                Error = urlChecked.Error,
                ErrorType = urlChecked.ErrorType,
            };
        }

        private static ResultData CreateResult(string url, HttpResponseMessage responseMessage)
        {
            ResultData result = new ResultData()
            {
                Success = true,
                Url = url,
                StatusCode = (int)responseMessage.StatusCode,
                Content = responseMessage.Content,
                Headers = responseMessage.Headers
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
