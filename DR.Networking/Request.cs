using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;

namespace DR.Networking
{
    public class Request
    {
        private static readonly HttpClient s_client = new HttpClient();
        private static HttpContent s_content { get; set; }
        private static HttpResponseHeaders s_headers { get; set; }

        private enum RequestTypes
        {
            Get,
            Post
        }

        /// <summary>Make a get request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a get request to</param>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Get(string url)
        {
            return await MakeRequest(url, RequestTypes.Get);
        }

        /// <summary>Make a get request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="headers">Your headers</param>
        /// <example>
        ///		<code>		
        ///			var content = new Dictionary<string, string>
        ///			{
        ///				{ "permission", "user" },
        ///				{ "permission_description", "general-user-account" }
        ///			};
        ///	
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(postUrl, content).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Get(string url, Dictionary<string, string> headers)
        {
            return await MakeRequest(url, RequestTypes.Get, null, headers);
        }

        /// <summary>Make a get request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a get request to</param>
        /// <param name="authHeader">Pass a auth header</param>
        /// <example>
        ///		<code>		
        ///			AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("password");
        ///	
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(postUrl, authHeader).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the post request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Get(string url, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, null, authHeader);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <example>
        ///		<code>		
        ///			var content = new FormUrlEncodedContent(new[]
        ///			{
        ///				new KeyValuePair<string, string>("permission", "user"),
        ///				new KeyValuePair<string, string>("permission_description", "general-user-account")
        ///			});
        ///	
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, FormUrlEncodedContent body)
        {
            return await MakeRequest(url, RequestTypes.Post, body);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <param name="authHeader">Pass a auth header</param>
        /// <example>
        ///		<code>		
        ///			var content = new FormUrlEncodedContent(new[]
        ///			{
        ///				new KeyValuePair<string, string>("permission", "user"),
        ///				new KeyValuePair<string, string>("permission_description", "general-user-account")
        ///			});
        ///			AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("password");
        ///	
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content, authHeader).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the post request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, FormUrlEncodedContent body, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, body, authHeader);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <param name="headers">Pass headers with the request</param>
        /// <example>
        ///		<code>		
        ///			var content = new FormUrlEncodedContent(new[]
        ///			{
        ///				new KeyValuePair<string, string>("permission", "user"),
        ///				new KeyValuePair<string, string>("permission_description", "general-user-account")
        ///			});
        ///			
        ///			var headers = new Dictionary<string, string>
        ///			{
        ///				{ "date-header", "10-09-2022" },
        ///				{ "time-header", "19:50" }
        ///			};
        ///	
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content, headers).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the post request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, FormUrlEncodedContent body, Dictionary<string, string> headers)
        {
            return await MakeRequest(url, RequestTypes.Post, body, headers);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <example>
        ///		<code>		
        ///			public partial class permissions
        ///			{
        ///				public string permission { get; set; }
        ///				public string permission_description { get; set; }
        ///			}
        ///	
        ///			permissions content = new permissions { permission = "user", permission_description = "general-user-account" };
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, dynamic body)
        {
            return await MakeRequest(url, RequestTypes.Post, body);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <param name="authHeader">Pass a auth header</param>
        /// <example>
        ///		<code>		
        ///			public partial class permissions
        ///			{
        ///				public string permission { get; set; }
        ///				public string permission_description { get; set; }
        ///			}
        ///	
        ///			permissions content = new permissions { permission = "user", permission_description = "general-user-account" };
        ///			AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("password");
        ///			
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content, authHeader).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, dynamic body, AuthenticationHeaderValue authHeader)
        {
            return await MakeRequest(url, RequestTypes.Post, body, authHeader);
        }

        /// <summary>Make a post request to a address</summary>
        /// <param name="url">The url (either DNS or IPv4) that you want to make a post request to</param>
        /// <param name="body">Body content that you want to post</param>
        /// <param name="headers">Pass headers with the request</param>
        /// <example>
        ///		<code>		
        ///			public partial class permissions
        ///			{
        ///				public string permission { get; set; }
        ///				public string permission_description { get; set; }
        ///			}
        ///			
        ///         var headers = new Dictionary<string, string>
        ///			{
        ///				{ "date-header", "10-09-2022" },
        ///				{ "time-header", "19:50" }
        ///			};
        ///	
        ///			permissions content = new permissions { permission = "user", permission_description = "general-user-account" };
        ///			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(postUrl, content, headers).Result;
        ///		</code>
        /// </example>
        /// <returns>
        ///		<description>(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)</description>
        ///     <list type="number">
        ///         <item>
        ///             <description>"result" bool. Result of the request</description>
        ///         </item>
        ///         <item>
        ///             <description>"errorCode" string. If result is false this returns the error code associated with why the request failed</description>
        ///         </item>
        ///         <item>
        ///             <description>"content" HttpContent. The content of the request (if result is true)</description>
        ///         </item>
        ///         <item>
        ///				<description>"headers" HttpResponseHeaders. Headers of the request</description>
        ///			</item>
        ///     </list>
        /// </returns>
        public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, dynamic body, Dictionary<string, string> headers)
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
        /// <returns></returns>
        private static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> MakeRequest(string url, RequestTypes request, dynamic body = null, dynamic headerValues = null)
        {
            (bool result, string error) = Core.Base.CheckUrl(url, out Uri requestUrl);
            if (result)
            {
                await Core.Base.RateLimit(requestUrl);

                if (headerValues != null)
                {
                    if (headerValues is AuthenticationHeaderValue)
                    {
                        s_client.DefaultRequestHeaders.Authorization = headerValues;
                    }
                    else // headerValues is Dictionary<string, string>
                    {
                        foreach (KeyValuePair<string, string> item in headerValues as Dictionary<string, string>)
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
                        {
                            response = await s_client.PostAsync(requestUrl, body);
                        }
                        else
                        {
                            body = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                            response = await s_client.PostAsync(requestUrl, body);
                        }
                        break;
                }

                s_content = response.Content;
                s_headers = response.Headers;

                if (!Core.Base.ResponseStatusMessage(response.StatusCode, out string errorCode))
                {
                    return (false, string.Format(Core.Base.s_errorLayout, requestUrl.AbsoluteUri, $"Something went wrong while making the request.\nStatus code: {(int)response.StatusCode}\nExplanation: {errorCode}"), s_content, s_headers);
                }
                else
                {
                    return (true, null, s_content, s_headers);
                }
            }
            else
            {
                return (false, error, s_content, s_headers);
            }
        }
    }
}
