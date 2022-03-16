using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Nager.PublicSuffix;

using DR.Networking.Core.Extensions;
using static DR.Networking.Core.Errors;
using static DR.Networking.Configuration;

namespace DR.Networking.Core
{
	public class Base
	{
		/// <summary>
		/// Indicates if Uri leads to a page or domain (google.com/hello or google.com/)
		/// </summary>
		internal enum UrlType
        {
			Page,
			Domain
        }

		/// <summary>
		/// Http protocols
		/// </summary>
		enum Protocol
		{
			Https,
			Http
		}

		internal static string s_errorLayout = $"[{DateTime.UtcNow}] " + "[DR.Networking] [{0}] Error: {1}";

		/// <summary>
		/// Creates a well formed uri
		/// </summary>
		/// <param name="url">Url you want to parse into a Uri</param>
		/// <param name="newUrl">New Uri which the URL was converted into</param>
		/// <param name="updateSiteSpecificList">(Optional) Indicate if the call was made by the UpdateSiteSpecificList method (tue for yes false for no)</param>
		/// <returns>
		///		<list type="number">
		///			<item>
		///				<para>Bool indicating if Uri conversion was successful</para>
		///			</item>
		///			<item>
		///				<para>String containing the error if one occured</para>
		///			</item>
		///		</list>
		/// </returns>
		/// <exception cref="GenericInvalidUrlError"></exception>
		/// <exception cref="Exception"></exception>
		internal static (bool result, string error) CheckUrl(string url, out Uri newUrl, bool updateSiteSpecificList = false)
		{
			newUrl = null;
			if (IPAddress.TryParse(url, out IPAddress address))
			{
				string error = "Provided {0} is of an unsuported type";
				error = string.Format(error, "address");
				switch (address.AddressFamily)
				{
					case System.Net.Sockets.AddressFamily.InterNetwork: //IPv4
						if (Uri.TryCreate(CheckHttpProtocol(url, Protocol.Http), UriKind.RelativeOrAbsolute, out newUrl))
						{
							return (true, null);
						}
						break;
					case System.Net.Sockets.AddressFamily.InterNetworkV6: //IPv6
						return (false, string.Format(s_errorLayout, address, $"{string.Format(error, "address")} (IPv6)"));
					default:
						return (false, string.Format(s_errorLayout, address, string.Format(error, "address")));
				}
			}
			else
			{
				if (Uri.TryCreate(CheckHttpProtocol(url), UriKind.RelativeOrAbsolute, out newUrl))
				{
					DomainParser domainParser = new DomainParser(new WebTldRuleProvider());
                    DomainInfo domainName;
                    try
                    {
						domainName = domainParser.Parse(newUrl.Host);
					}
					catch (Exception ex)
                    {
						if (ex is ParseException)
                        {
							if (updateSiteSpecificList)
								throw new GenericInvalidUrlError($"The URL you provided isn't valid.\n" +
									$"Make sure all the URLs you provided in your 'DR.Networking.Configuration' are valid.\n" +
									$"Url: {url}");
							return (false, string.Format(s_errorLayout, url, "The url you provided isn't valid"));
						}
						else
                        {
							throw new Exception($"Something went wrong while parsing your URL.\nUrl: {newUrl}\nError: {ex}");
						}						
                    }

					if (domainName.TLD.Length > 0)
					{
						try
						{
							//See if "URL" has a valid host entry
							Dns.GetHostEntry(domainName.RegistrableDomain);
							return (true, null);
						}
						catch
						{
							return (false, string.Format(s_errorLayout, url, "Url does not have a valid host entry"));
						}
					}
					return (false, string.Format(s_errorLayout, url, "The provided URL does not have a valid top level domain"));
				}
				else
				{
					return (false, string.Format(s_errorLayout, address, $"The provided URL/address isn't supported"));
				}
			}
			return (false, null);
		}

		/// <summary>
		/// Adds the http protocol to a URL
		/// </summary>
		/// <param name="url">The url for which you want to check the Http protocol</param>
		/// <param name="protocol">(Optional) What Http protocol was used for the request</param>
		/// <returns></returns>
		private static string CheckHttpProtocol(string url, Protocol? protocol = null)
		{
			string newUri;
			switch (url)
			{
				case string a when a.StartsWith("https://"):
				case string b when b.StartsWith("http://"):
					newUri = url;
					break;
				default:
					switch (protocol)
					{
						case Protocol.Http:
						default:
							newUri = "http://" + url;
							break;
						case Protocol.Https:
							newUri = "https://" + url;
							break;
					}
					break;
			}
			return newUri;
		}

		/// <summary>
		/// Returns an error message when a bad status code is encountered
		/// </summary>
		/// <param name="code">The HttpStatusCode of the request made</param>
		/// <param name="errorMsg">The error message associated with the HttpStatusCode. Null if HttpStatusCode indicates request was successful</param>
		/// <returns>
		///		A bool indicating if http request was successful
		/// </returns>
		internal static bool ResponseStatusMessage(HttpStatusCode code, out string errorMsg)
		{
			//Return an explanation message with HTTP status codes so it's more clear what went wrong.
			switch (code)
			{
				case HttpStatusCode.BadRequest:
					errorMsg = "BadRequest indicates that the request could not be understood by the server. BadRequest is sent when no other error is applicable, or if the exact error is unknown or does not have its own error code.";
					return false;

				case HttpStatusCode.NotFound:
					errorMsg = "NotFound indicates that the requested resource does not exist on the server.";
					return false;

				case HttpStatusCode.InternalServerError:
					errorMsg = "InternalServerError indicates that a generic error has occurred on the server.";
					return false;

				case HttpStatusCode.ServiceUnavailable:
					errorMsg = "ServiceUnavailable indicates that the server is temporarily unavailable, usually due to high load or maintenance.";
					return false;

				case HttpStatusCode.BadGateway:
					errorMsg = "BadGateway indicates that an intermediate proxy server received a bad response from another proxy or the origin server.";
					return false;

				case HttpStatusCode.GatewayTimeout:
					errorMsg = "GatewayTimeout indicates that an intermediate proxy server timed out while waiting for a response from another proxy or the origin server.";
					return false;

				case HttpStatusCode.Conflict:
					errorMsg = "Conflict indicates that the request could not be carried out because of a conflict on the server.";
					return false;

				case HttpStatusCode.ExpectationFailed:
					errorMsg = "ExpectationFailed indicates that an expectation given in an Expect header could not be met by the server.";
					return false;

				case HttpStatusCode.FailedDependency:
					errorMsg = "FailedDependency indicates that the method couldn't be performed on the resource because the requested action depended on another action and that action failed.";
					return false;

				case HttpStatusCode.Forbidden:
					errorMsg = "Forbidden indicates that the server refuses to fulfill the request.";
					return false;

				case HttpStatusCode.Gone:
					errorMsg = "Gone indicates that the requested resource is no longer available.";
					return false;

				case HttpStatusCode.HttpVersionNotSupported:
					errorMsg = "HttpVersionNotSupported indicates that the requested HTTP version is not supported by the server.";
					return false;

				case HttpStatusCode.InsufficientStorage:
					errorMsg = "InsufficientStorage indicates that the server is unable to store the representation needed to complete the request.";
					return false;

				case HttpStatusCode.LengthRequired:
					errorMsg = "LengthRequired indicates that the required Content-length header is missing.";
					return false;

				case HttpStatusCode.Locked:
					errorMsg = "Locked indicates that the source or destination resource is locked.";
					return false;

				case HttpStatusCode.MethodNotAllowed:
					errorMsg = "MethodNotAllowed indicates that the request method (POST or GET) is not allowed on the requested resource.";
					return false;

				case HttpStatusCode.MisdirectedRequest:
					errorMsg = "MisdirectedRequest indicates that the request was directed at a server that is not able to produce a response.";
					return false;

				case HttpStatusCode.NotExtended:
					errorMsg = "NotExtended indicates that further extensions to the request are required for the server to fulfill it.";
					return false;

				case HttpStatusCode.NotImplemented:
					errorMsg = "NotImplemented indicates that the server does not support the requested function.";
					return false;

				case HttpStatusCode.PreconditionFailed:
					errorMsg = "PreconditionFailed indicates that a condition set for this request failed, and the request cannot be carried out. Conditions are set with conditional request headers like If-Match, If-None-Match, or If-Unmodified-Since.";
					return false;

				case HttpStatusCode.PreconditionRequired:
					errorMsg = "PreconditionRequired indicates that the server requires the request to be conditional.";
					return false;

				case HttpStatusCode.RequestedRangeNotSatisfiable:
					errorMsg = "RequestedRangeNotSatisfiable indicates that the range of data requested from the resource cannot be returned, either because the beginning of the range is before the beginning of the resource, or the end of the range is after the end of the resource.";
					return false;

				case HttpStatusCode.RequestEntityTooLarge:
					errorMsg = "RequestEntityTooLarge indicates that the request is too large for the server to process.";
					return false;

				case HttpStatusCode.RequestHeaderFieldsTooLarge:
					errorMsg = "RequestHeaderFieldsTooLarge indicates that the server is unwilling to process the request because its header fields (either an individual header field or all the header fields collectively) are too large.";
					return false;

				case HttpStatusCode.RequestTimeout:
					errorMsg = "RequestTimeout indicates that the client did not send a request within the time the server was expecting the request.";
					return false;

				case HttpStatusCode.RequestUriTooLong:
					errorMsg = "RequestUriTooLong indicates that the URI is too long.";
					return false;

				case HttpStatusCode.TooManyRequests:
					errorMsg = "TooManyRequests indicates that the user has sent too many requests in a given amount of time.";
					return false;

				case HttpStatusCode.Unauthorized:
					errorMsg = "Unauthorized indicates that the requested resource requires authentication. The WWW-Authenticate header contains the details of how to perform the authentication.";
					return false;

				case HttpStatusCode.UnavailableForLegalReasons:
					errorMsg = "UnavailableForLegalReasons indicates that the server is denying access to the resource as a consequence of a legal demand.";
					return false;

				case HttpStatusCode.UnprocessableEntity:
					errorMsg = "UnprocessableEntity indicates that the request was well-formed but was unable to be followed due to semantic errors.";
					return false;

				case HttpStatusCode.UnsupportedMediaType:
					errorMsg = "UnsupportedMediaType indicates that the request is an unsupported type.";
					return false;

				case HttpStatusCode.UseProxy:
					errorMsg = "UseProxy indicates that the request should use the proxy server at the URI specified in the Location header.";
					return false;

				default:
					errorMsg = null;
					return true;
			}
		}

		/// <summary>
		/// Updates the siteList. Checks wether the specified urls are for a page or a domain.
		/// </summary>
		/// <param name="siteList"></param>
		/// <returns>Updated List</returns>
		/// <exception cref="RateLimitingUrlNotValid"></exception>
		internal static List<SiteSpecific> UpdateSiteSpecificList(List<SiteSpecific> siteList)
		{
			foreach (SiteSpecific item in siteList)
            {
				if (CheckUrl(item.Url, out Uri url, true).result)
                {
					UrlType urlType;
					switch (url.AbsolutePath)
                    {
						case "/":
							urlType = UrlType.Domain;
							break;
						default:
							urlType = UrlType.Page;
							break;
                    }
					item._urlType = urlType;
					item._uri = url;
				}
				else
                {
					throw new RateLimitingUrlNotValid($"One of the urls you provided for domain/url specific rate limiting is not a valid url: {item.Url}");
                }
            }
			return siteList;
		}

		/// <summary>
		/// Function that throttles network calls.
		/// Specifically it throttles calls according to the settings applied in the Configuration class.
		/// </summary>
		/// <param name="requestUrl">Url to which a network called is being made</param>
		internal static async Task RateLimit(Uri requestUrl)
        {
			if (PerSites != null)
            {
				(bool result, SiteSpecific item) getSite = PerSites.FindUri(requestUrl);
				(bool result, RequestData request, UrlType? type) getRequest = s_requestCollection.FindUri(requestUrl);

				if (getSite.result)
				{
					if (getRequest.result)
					{
						await RateLimit(getSite.item.Duration.TotalMilliseconds, getRequest.request._time);
					}

					await UpdateCollection(requestUrl);
				}
				else
				{
					if (Global != null)
					{
						if (getRequest.result)
						{
							await RateLimit(Global.Value.TotalMilliseconds, getRequest.request._time);
						}

						await UpdateCollection(requestUrl);
					}
				}
			}
		}

		/// <summary>
		/// Sleeps the current thread for a specific duration
		/// </summary>
		/// <param name="duration">The duration of the ratelimit (how much time there should be between calls)</param>
		/// <param name="previousRequest">DateTime of the previous request</param>
		/// <returns></returns>
		private static Task RateLimit(double duration, DateTime previousRequest)
        {
			double remainingDifference = 0;
			double previousRequestDifference = (DateTime.UtcNow - previousRequest).TotalMilliseconds;
			
			if (duration > previousRequestDifference)
				remainingDifference = duration - previousRequestDifference;
			
			Thread.Sleep((int)remainingDifference);

			return Task.CompletedTask;
        }

		/// <summary>
		/// Removes old items from the s_requestCollection (if they exist) and add new ones.
		/// </summary>
		/// <param name="uri">Uri to which a network request was made</param>
		/// <returns>A completed task</returns>
		private static Task UpdateCollection(Uri uri)
        {
            (bool result, RequestData data, UrlType? type) result = s_requestCollection.FindUri(uri);
			if (result.result)
            {
				s_requestCollection.Remove(result.data);
			}

			RequestData responseData = new RequestData()
			{
				_time = DateTime.UtcNow,
				_url = uri,
			};
			s_requestCollection.Add(responseData);

			return Task.CompletedTask;
		}
	}
}
