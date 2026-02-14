using DR.Networking.Core.Attributes;
using DR.Networking.Models;
using Easy.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
        [Supported(true)]
        Head,
        [Supported(true)]
        Post,
        [Supported(true)]
        Put,
        [Supported(true)]
        Delete,
        [Supported(true)]
        Trace,
        [Supported(true)]
        Options,
        [Supported(true)]
        Connect,
        [Supported(true)]
        Patch,
        [Supported(false)]
        Unknown
    }

    internal class Main
    {
        /// <summary>
        /// A list of <see cref="RequestTypes"/> that are supported by the library.
        /// </summary>
        internal static readonly List<RequestTypes> SupportedTypes = GetSupportedTypes();

        /// <summary>
        /// Gets the singleton instance of the IDGenerator used for generating unique identifiers.
        /// </summary>
        /// <remarks>This static field ensures that all parts of the application use the same instance of <see cref="IDGenerator"/>.</remarks>
        private static readonly IDGenerator _generator = IDGenerator.Instance;

        /// <summary>
        /// The base function for making a network request to a specific url.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="namedClient"></param>
        /// <returns></returns>
        internal static async Task<Result> RequestBase<T>(HttpRequestMessage request, string? namedClient = null)
        {
            Result result;
            HttpClient client;

            if (string.IsNullOrWhiteSpace(namedClient))
                client = Settings.Client;
            else
                client = Settings.NamedClients.FirstOrDefault(x => x.Name == namedClient)?.Client ?? Settings.Client;

            Uri? uri = BuildUri(client.BaseAddress, request.RequestUri);
            string url = uri?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(url))
            {
                result = new Result()
                {
                    Success = false,
                    Url = string.Empty,
                    Error = "No url provided",
                    ErrorType = ErrorType.InvalidUrl,
                };

                await result.CloneRequestMessage(request);
                return result;
            }

            if (!request.Method.IsSupported())
            {
                result = new Result()
                {
                    Success = false,
                    Url = url,
                    Error = $"The selected HttpMethod '{request.Method}' is either not supported or not yet implemented",
                    ErrorType = ErrorType.HttpMethodNotSupported,
                };

                await result.CloneRequestMessage(request);
                return result;
            }

            if (Settings.ValidateUrl)
            {
                CheckUrlModel urlChecked = await Base.CheckUrl(url);
                if (urlChecked.Success)
                {
                    // Since we fixed any problems with the url in the 'CheckUrl' function we will now use this full url.
                    request.RequestUri = urlChecked.Url;
                }
                else
                {
                    result = new Result()
                    {
                        Url = GetResultUrl(urlChecked.Url, url),
                        Error = urlChecked.Error,
                        ErrorType = urlChecked.ErrorType,
                    };

                    await result.CloneRequestMessage(request);
                    return result;
                }
            }
            else
                request.RequestUri = uri;

            string id = _generator.Next;
            long start = Stopwatch.GetTimestamp();

            History.Add(new()
            {
                Id = id,
                Start = start,
                Url = request.RequestUri,
            });

            RateLimitResult ratelimit = await new RateLimiter(id, start).Check(request.RequestUri);
            if (ratelimit.TimedOut)
            {
                History.Update(new HistoryData(History.Get(id))
                {
                    End = Stopwatch.GetTimestamp(),
                });

                result = new Result()
                {
                    Success = false,
                    Url = request.RequestUri?.ToString() ?? string.Empty,
                    Error = $"Request was in the rate limit queue for more than {ratelimit.Timeout}",
                    ErrorType = ErrorType.RateLimitTimeout,
                };

                await result.CloneRequestMessage(request);
                return result;
            }

            HttpResponseMessage response = await client.SendAsync(request);
            History.Update(new HistoryData(History.Get(id))
            {
                End = Stopwatch.GetTimestamp(),
            });

            result = new()
            {
                Response = response,
            };

            await result.CloneRequestMessage(request);
            return result;
        }

        /// <summary>
        /// Try and build one valid <see cref="Uri"/> from the 'HttpClient.BaseAddress' & 'HttpRequestMessage.RequestUri'.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        private static Uri? BuildUri(Uri? baseAddress, Uri? requestUri)
        {
            if (baseAddress == null && requestUri == null)
                return null;

            // Don't use the 'baseAddress' if the 'requestUri' is already an absolute uri.
            if (requestUri != null && requestUri.IsAbsoluteUri)
                return requestUri;

            if (baseAddress != null)
            {
                if (requestUri != null)
                    return new Uri(baseAddress, requestUri);
                else
                    return baseAddress;
            }

            return requestUri;
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

        private static List<RequestTypes> GetSupportedTypes()
        {
            List<RequestTypes> result = [];
            Type rType = typeof(RequestTypes);
            Type sType = typeof(Supported);

            IEnumerable<MemberInfo> members = rType.GetMembers()
                .Where(x => x.DeclaringType == rType && x.GetCustomAttributes(sType, false).Length > 0);

            foreach (MemberInfo member in members)
            {
                object? attr = member.GetCustomAttributes(sType, false).FirstOrDefault();
                if (attr != null && ((Supported)attr).IsSupported)
                {
                    if (Enum.TryParse(member.Name, true, out RequestTypes supported) && !result.Contains(supported))
                        result.Add(supported);
                }
            }

            return result;
        }
    }
}
