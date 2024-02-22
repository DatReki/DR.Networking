using DR.Networking.Models;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal class Base
    {
        /// <summary>
        /// Check if a URL has a valid format.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static async Task<CheckUrlModel> CheckUrl(string url)
        {
            // When the URL is a IPv4 or IPv6 address check if it's valid.
            if (IPAddress.TryParse(url, out IPAddress address))
            {
                (_, string protocolValue) = GetHttpProtocol(url);
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork: // IPv4
                        if (Uri.TryCreate(protocolValue + address.ToString(), UriKind.RelativeOrAbsolute, out Uri newUrl))
                            return new CheckUrlModel(true, newUrl, null);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv4 address.");
                    case AddressFamily.InterNetworkV6: // IPv6
                        if (Uri.TryCreate(protocolValue + $"[{address}]", UriKind.RelativeOrAbsolute, out newUrl))
                            return new CheckUrlModel(true, newUrl, null);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv6 address.");
                    default:
                        return new CheckUrlModel(false, null, "Was unable to parse url to either a IPv4 or IPv6 address.");
                }
            }
            // When the URL is a Uri check if it's valid.
            else if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri newUrl))
            {
                LocalFileRuleProvider ruleProvider = new LocalFileRuleProvider("public_suffix_list.dat");
                await ruleProvider.BuildAsync();

                DomainParser domainParser = new DomainParser(ruleProvider);

                if (domainParser.IsValidDomain(url))
                {
                    DomainInfo? domainInfo = domainParser.Parse(url);

                    // Technically a redundant check.
                    if (domainInfo == null)
                        return new CheckUrlModel(false, null, "The URL you provided is not a fully qualified domain name (FQDN).");

                    try
                    {
                        Dns.GetHostEntry(domainInfo.RegistrableDomain);
                        return new CheckUrlModel(true, newUrl, null);
                    }
                    catch (Exception ex)
                    {
                        if (ex is ArgumentOutOfRangeException)
                            return new CheckUrlModel(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is longer than 255 characters.");
                        else if (ex is SocketException)
                            return new CheckUrlModel(false, null, $"Encountered an error when trying to resolve the hostname ({domainInfo.RegistrableDomain}).");
                        else if (ex is ArgumentException)
                            return new CheckUrlModel(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is invalid.");
                        else
                            return new CheckUrlModel(false, null, $"Something went wrong  while trying to parse the hostname ({domainInfo.RegistrableDomain}).");
                    }
                }
                else
                    return new CheckUrlModel(false, null, "The URL you provided is not a fully qualified domain name (FQDN).");
            }
            else
                return new CheckUrlModel(false, null, "Was unable to parse either a valid URL or a IPv4/IPv6 address.");
        }

        /// <summary>
        /// Get the HTTP protocol used in the url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static (Protocol protocol, string value) GetHttpProtocol(string url)
        {
            string http = "http://";
            string https = "https://";

            switch (url)
            {
                case string a when a.StartsWith(http):
                    return (Protocol.Http, http);
                case string b when b.StartsWith(https):
                    return (Protocol.Https, https);
                default:
                    if (Settings.UseHttpsByDefault)
                        return (Protocol.Https, https);
                    else
                        return (Protocol.Http, http);
            }
        }
    }
}
