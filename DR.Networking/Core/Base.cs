using DR.Networking.Models;
using Nager.PublicSuffix;
using Nager.PublicSuffix.RuleProviders;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal class Base
    {
        internal static long SuffixTimestamp { get; private set; } = 0;
        internal static BaseRuleProvider? RuleProvider { get; private set; }
        internal static DomainParser? DomainParser { get; private set; }

        internal enum HostType
        {
            Unknown,
            Domain,
            Ip,
        }

        /// <summary>
        /// Check if a URL has a valid format.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        internal static async Task<UrlCheck> CheckUrl(string url)
        {
            UrlHistory? history = History.Urls.FirstOrDefault(x => x.Original == url);
            if (history != null && history.Checked != null && history.Checked?.Success == true)
                return history.Checked;

            if (TryCreateUri(url, UriKind.Absolute, out Uri? newUrl) && newUrl != null)
            {
                switch (newUrl.HostNameType)
                {
                    case UriHostNameType.IPv4:
                    case UriHostNameType.IPv6:
                        return new UrlCheck(true, newUrl, null, ErrorType.None);
                    case UriHostNameType.Basic:
                    case UriHostNameType.Dns:
                        {
                            // Ignore domain check for localhost requests.
                            if (newUrl.Host == "localhost")
                                return new UrlCheck(true, newUrl, null, ErrorType.None);

                            DomainParser domainParser = await GetDomainParser();
                            if (domainParser.IsValidDomain(newUrl.Host))
                            {
                                DomainInfo? domainInfo = domainParser.Parse(newUrl.Host);

                                // Technically a redundant check.
                                if (domainInfo == null)
                                    return new UrlCheck(false, null, "The URL you provided is not a fully qualified domain name (FQDN).", ErrorType.InvalidDomain);

                                try
                                {
                                    if (string.IsNullOrWhiteSpace(domainInfo.RegistrableDomain))
                                        return new UrlCheck(false, null, "The provided hostname is empty.", ErrorType.InvalidHostname);

                                    Dns.GetHostEntry(domainInfo.RegistrableDomain);
                                    return new UrlCheck(true, newUrl, null, ErrorType.None);
                                }
                                catch (Exception ex)
                                {
                                    ErrorType errorType = ErrorType.InvalidHostname;

                                    if (ex is ArgumentOutOfRangeException)
                                        return new UrlCheck(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is longer than 255 characters.", errorType);
                                    else if (ex is SocketException)
                                        return new UrlCheck(false, null, $"Encountered an error when trying to resolve the hostname ({domainInfo.RegistrableDomain}).", errorType);
                                    else if (ex is ArgumentException)
                                        return new UrlCheck(false, null, $"The provided hostname ({domainInfo.RegistrableDomain}) is invalid.", errorType);
                                    else
                                        return new UrlCheck(false, null, $"Something went wrong  while trying to parse the hostname ({domainInfo.RegistrableDomain}).", errorType);
                                }
                            }
                            else
                                return new UrlCheck(false, null, "The URL you provided is not a fully qualified domain name (FQDN).", ErrorType.InvalidDomain);
                        }
                    default:
                        return new UrlCheck(false, null, "Was unable to parse either a valid URL or a IPv4/IPv6 address.", ErrorType.InvalidUrl);
                }
            }
            else
                return new UrlCheck(false, null, "Was unable to parse either a valid URL or a IPv4/IPv6 address.", ErrorType.InvalidUrl);
        }

        /// <summary>
        /// Try and create a valid <see cref="Uri"/>
        /// </summary>
        /// <param name="url"></param>
        /// <param name="kind"></param>
        /// <param name="uri"></param>
        /// <param name="hostType">If the host of the new <see cref="Uri"/> is a domain or a <see cref="IPAddress"/></param>
        /// <returns></returns>
        private static bool TryCreateUri(string url, UriKind kind, out Uri? uri)
        {
            if (kind == UriKind.Absolute)
            {
                // Turn a IPV6 address into a valid url.
                if (IPAddress.TryParse(url, out IPAddress? address) && address != null && address.AddressFamily == AddressFamily.InterNetworkV6)
                    url = $"[{url}]";

                string http = "http://";
                string https = "https://";

                if (!url.StartsWith(http) && !url.StartsWith(https))
                {
                    if (Settings.UseHttpsByDefault)
                        url = https + url;
                    else
                        url = http + url;
                }
            }

            return Uri.TryCreate(url, kind, out uri);
        }

        /// <summary>
        /// Get the <see cref="Nager.PublicSuffix.DomainParser"/>.<br />
        /// Automatically tries to download the newest public suffix list once per day.<br />
        /// If it's unable to download any it will use the file provided by the library.
        /// </summary>
        /// <returns></returns>
        private static async Task<DomainParser> GetDomainParser()
        {
            if ((DomainParser == null || RuleProvider == null) || SuffixTimestamp == 0 || Tools.Stopwatch.GetElapsedTime(SuffixTimestamp).TotalDays >= 1)
            {
                var httpProvider = new SimpleHttpRuleProvider();
                bool build = await httpProvider.BuildAsync();
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
                SuffixTimestamp = Stopwatch.GetTimestamp();
            }

            return DomainParser;
        }
    }
}
