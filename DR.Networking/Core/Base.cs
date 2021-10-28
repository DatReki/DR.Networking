using Nager.PublicSuffix;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DR.Networking.Core
{
	public class Base
	{
		internal static string ErrorLayout = $"[{DateTime.UtcNow}] " + "[DR.Networking] [{0}] Error: {1}";

		/// <summary>
		/// Creates a well formed uri
		/// </summary>
		internal static (bool result, string error) CheckUrl(string url, out Uri newUrl)
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
						return (false, string.Format(ErrorLayout, address, $"{string.Format(error, "address")} (IPv6)"));
					default:
						return (false, string.Format(ErrorLayout, address, string.Format(error, "address")));
				}
			}
			else
			{
				if (Uri.TryCreate(CheckHttpProtocol(url), UriKind.RelativeOrAbsolute, out newUrl))
				{
					DomainParser domainParser = new DomainParser(new WebTldRuleProvider());
					DomainInfo domainName = domainParser.Parse(newUrl.Host);

					if (domainName.TLD.Length > 0)
					{
						try
						{
							//See if "url" has a valid host entry
							Dns.GetHostEntry(domainName.RegistrableDomain);
							return (true, null);
						}
						catch
						{
							return (false, string.Format(ErrorLayout, url, "Url does not have a valid host entry"));
						}
					}
					return (false, string.Format(ErrorLayout, url, "The provided url does not have a valid top level domain"));
				}
				else
				{
					return (false, string.Format(ErrorLayout, address, $"The provided url/address isn't supported"));
				}
			}
			return (false, null);
		}

		private enum Protocol
		{
			Https,
			Http
		}

		/// <summary>
		/// Adds the http protocol to a url
		/// </summary>
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
	}
}
