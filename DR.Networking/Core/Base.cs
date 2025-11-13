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
        internal static DateTime SuffixUpdated { get; private set; } = DateTime.MinValue;
        internal static BaseRuleProvider? RuleProvider { get; private set; }
        internal static DomainParser? DomainParser { get; private set; }

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
                            return new CheckUrlModel(true, newUrl, null, ErrorType.None);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv4 address.", ErrorType.InvalidIpAddress);
                    case AddressFamily.InterNetworkV6: // IPv6
                        if (Uri.TryCreate(protocolValue + $"[{address}]", UriKind.RelativeOrAbsolute, out newUrl))
                            return new CheckUrlModel(true, newUrl, null, ErrorType.None);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv6 address.", ErrorType.InvalidIpAddress);
                    default:
                        return new CheckUrlModel(false, null, "Was unable to parse url to either a IPv4 or IPv6 address.", ErrorType.InvalidIpAddress);
                }
            }
            // When the URL is a Uri check if it's valid.
            else if (Uri.TryCreate(url, UriKind.Absolute, out Uri newUrl))
            {
                DomainParser domainParser = await GetDomainParser();
                if (domainParser.IsValidDomain(newUrl.Host))
                {
                    DomainInfo? domainInfo = domainParser.Parse(newUrl.Host);

                    // Technically a redundant check.
                    if (domainInfo == null)
                        return new CheckUrlModel(false, null, "The URL you provided is not a fully qualified domain name (FQDN).", ErrorType.InvalidDomain);

                    try
                    {
                        Dns.GetHostEntry(domainInfo.RegistrableDomain);
                        return new CheckUrlModel(true, newUrl, null, ErrorType.None);
                    }
                    catch (Exception ex)
                    {
                        ErrorType errorType = ErrorType.InvalidHostname;

                        if (ex is ArgumentOutOfRangeException)
                            return new CheckUrlModel(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is longer than 255 characters.", errorType);
                        else if (ex is SocketException)
                            return new CheckUrlModel(false, null, $"Encountered an error when trying to resolve the hostname ({domainInfo.RegistrableDomain}).", errorType);
                        else if (ex is ArgumentException)
                            return new CheckUrlModel(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is invalid.", errorType);
                        else
                            return new CheckUrlModel(false, null, $"Something went wrong  while trying to parse the hostname ({domainInfo.RegistrableDomain}).", errorType);
                    }
                }
                else
                    return new CheckUrlModel(false, null, "The URL you provided is not a fully qualified domain name (FQDN).", ErrorType.InvalidDomain);
            }
            else
                return new CheckUrlModel(false, null, "Was unable to parse either a valid URL or a IPv4/IPv6 address.", ErrorType.InvalidUrl);
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

        /// <summary>
        /// Get the <see cref="Nager.PublicSuffix.DomainParser"/>.<br />
        /// Automatically tries to download the newest public suffix list once per day.<br />
        /// If it's unable to download any it will use the file provided by the library.
        /// </summary>
        /// <returns></returns>
        private static async Task<DomainParser> GetDomainParser()
        {
            if ((DomainParser == null || RuleProvider == null) || (DateTime.Now - SuffixUpdated).TotalDays >= 1)
            {
                var httpProvider = new SimpleHttpRuleProvider();
                var build = await httpProvider.BuildAsync();
                if (build)
                    RuleProvider = httpProvider;

                // Fall back method for if it's unable to build
                if (RuleProvider == null)
                {
                    var fileProvider = new LocalFileRuleProvider("public_suffix_list.dat");
                    await fileProvider.BuildAsync();

                    RuleProvider = fileProvider;
                }

                DomainParser = new DomainParser(RuleProvider);
                SuffixUpdated = DateTime.Now;
            }

            return DomainParser;
        }
    }
}
