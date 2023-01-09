using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DR.Networking
{
    public class Request
    {
        internal static HttpClient s_client = new HttpClient();

        private enum RequestTypes
        {
            Get,
            Post
        }

        /// <summary>
        /// Make a get request to a address.
        /// </summary>
        /// <param name="url">Url of the address you want to make a get request to.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Get(string url)
        {
            return await MakeRequest(url, RequestTypes.Get);
        }

        /// <summary>
        /// Make a get request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="headers">Your headers.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Get(string url, Dictionary<string, string> headers)
        {
            return await MakeRequest(url, RequestTypes.Get, null, headers);
        }

        /// <summary>
        /// Make a get request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a get request to.</param>
        /// <param name="authHeader">Pass a auth header.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Get(string url, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, null, authHeader);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, FormUrlEncodedContent body)
        {
            return await MakeRequest(url, RequestTypes.Post, body);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <param name="authHeader">Pass a auth header.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, FormUrlEncodedContent body, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, body, authHeader);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <param name="headers">Pass headers with the request.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, FormUrlEncodedContent body, Dictionary<string, string> headers)
        {
            return await MakeRequest(url, RequestTypes.Post, body, headers);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, dynamic body)
        {
            return await MakeRequest(url, RequestTypes.Post, body);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <param name="authHeader">Pass a auth header.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, dynamic body, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, body, authHeader);
        }

        /// <summary>
        /// Make a post request to a address.
        /// </summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to.</param>
        /// <param name="body">Body content that you want to post.</param>
        /// <param name="headers">Pass headers with the request.</param>
        /// <returns>Data about the result of the request.</returns>
        public static async Task<Data> Post(string url, dynamic body, Dictionary<string, string> headers)
        {
            return await MakeRequest(url, RequestTypes.Post, body, headers);
        }

        /// <summary>
        /// Base method to make the networking requests.
        /// </summary>
        /// <param name="url">The url you want to make the request to</param>
        /// <param name="request">The type of request being made</param>
        /// <param name="body">(Optional) post values</param>
        /// <param name="headerValues">headers you want to set with the request</param>
        /// <returns>Data about the result of the request.</returns>
        private static async Task<Data> MakeRequest(string url, RequestTypes request, dynamic? body = null, dynamic? headerValues = null)
        {
            Data result;
            s_client.DefaultRequestHeaders.Clear();

            (bool checkUrl, string error) = Core.Base.CheckUrl(url, out Uri requestUrl);
            if (checkUrl)
            {
                await Core.Base.RateLimit(requestUrl);

                if (headerValues != null)
                {
                    if (headerValues is AuthenticationHeaderValue)
                        s_client.DefaultRequestHeaders.Authorization = headerValues;
                    else
                    {
                        foreach (KeyValuePair<string, string> item in (Dictionary<string, string>)headerValues)
                        {
                            s_client.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }
                }

                HttpResponseMessage response;
                switch (request)
                {
                    case RequestTypes.Get:
                        response = await s_client.GetAsync(requestUrl);
                        break;
                    case RequestTypes.Post:
                    default:
                        if (body is FormUrlEncodedContent)
                            response = await s_client.PostAsync(requestUrl, body);
                        else
                        {
                            body = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                            response = await s_client.PostAsync(requestUrl, body);
                        }
                        break;
                }

                HttpResponseHeaders headers = response.Headers;
                HttpContent content = response.Content;

                if (Core.Base.ResponseStatusMessage(response.StatusCode, out string statusError))
                {
                    result = new Data()
                    {
                        Result = true,
                        Error = string.Empty,
                        Content = content,
                        Headers = headers
                    };
                }
                else
                {
                    result = new Data()
                    {
                        Result = false,
                        Error = string.Format(Core.Base.s_errorLayout, requestUrl.AbsoluteUri, $"Something went wrong while making the request.\nStatus code: {(int)response.StatusCode}\nExplanation: {statusError}"),
                        Content = content,
                        Headers = headers
                    };
                }
            }
            else
            {
                result = new Data()
                {
                    Result = false,
                    Error = error,
                    Content = null,
                    Headers = null
                };
            }
            return result;
        }
    }
}
