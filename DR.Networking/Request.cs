using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DR.Networking
{
	public class Request
	{
		private static HttpClient Client = new HttpClient();
		private static HttpContent content { get; set; }
		private static HttpResponseHeaders headers { get; set; }

		/// <summary>Make a get request to a address</summary>
		/// <param name="url">The url (either DNS or IPv4) that you want to make a get request too</param>
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
		///				<description>"headers" HttpResponseHeaders. Headers of the get request</description>
		///			</item>
		///     </list>
		/// </returns>
		public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Get(string url)
		{
			(bool result, string error) checkUrl = Core.Base.CheckUrl(url, out Uri requestUrl);
			if (checkUrl.result)
			{
				HttpResponseMessage response = await Client.GetAsync(requestUrl);
				content = response.Content;
				headers = response.Headers;
				if (!Core.Base.ResponseStatusMessage(response.StatusCode, out string errorCode))
				{
					return (false, string.Format(Core.Base.ErrorLayout, requestUrl.AbsoluteUri, $"Something went wrong while making the request.\nStatus code: {(int)response.StatusCode}\nExplanation: {errorCode}"), content, headers);
				}
				else
				{
					return (true, null, content, headers);
				}
			}
			else
			{
				return (false, checkUrl.error, content, headers);
			}
		}

		/// <summary>Make a get post request to a address</summary>
		/// <param name="url">The url (either DNS or IPv4) that you want to make a post request too</param>
		/// <param name="post">Content that you want to post</param>
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
		///				<description>"headers" HttpResponseHeaders. Headers of the get request</description>
		///			</item>
		///     </list>
		/// </returns>
		public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, FormUrlEncodedContent post)
		{
			(bool result, string error) checkUrl = Core.Base.CheckUrl(url, out Uri requestUrl);
			if (checkUrl.result)
			{
				HttpResponseMessage response = await Client.PostAsync(requestUrl, post);
				content = response.Content;
				headers = response.Headers;
				if (!Core.Base.ResponseStatusMessage(response.StatusCode, out string errorCode))
				{
					return (false, string.Format(Core.Base.ErrorLayout, requestUrl.AbsoluteUri, $"Something went wrong while making the request.\nStatus code: {(int)response.StatusCode}\nExplanation: {errorCode}"), content, headers);
				}
				else
				{
					return (true, null, content, headers);
				}
			}
			else
			{
				return (false, checkUrl.error, content, headers);
			}
		}

		/// <summary>Make a get post request to a address</summary>
		/// <param name="url">The url (either DNS or IPv4) that you want to make a post request too</param>
		/// <param name="post">Content that you want to post</param>
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
		///				<description>"headers" HttpResponseHeaders. Headers of the get request</description>
		///			</item>
		///     </list>
		/// </returns>
		public static async Task<(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers)> Post(string url, dynamic post)
		{
			(bool result, string error) checkUrl = Core.Base.CheckUrl(url, out Uri requestUrl);
			if (checkUrl.result)
			{
				var data = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await Client.PostAsync(requestUrl, data);
				content = response.Content;
				headers = response.Headers;
				if (!Core.Base.ResponseStatusMessage(response.StatusCode, out string errorCode))
				{
					return (false, string.Format(Core.Base.ErrorLayout, requestUrl.AbsoluteUri, $"Something went wrong while making the request.\nStatus code: {(int)response.StatusCode}\nExplanation: {errorCode}"), content, headers);
				}
				else
				{
					return (true, null, content, headers);
				}
			}
			else
			{
				return (false, checkUrl.error, content, headers);
			}
		}
	}
}
