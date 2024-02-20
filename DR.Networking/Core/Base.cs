using DR.Networking.Models;
using System;
using System.Net;
using System.Net.Sockets;

namespace DR.Networking.Core
{
    internal class Base
    {
        internal static CheckUrlModel CheckUrl(string url)
        {
            CheckUrlModel result = new CheckUrlModel();

            if (IPAddress.TryParse(url, out IPAddress address))
            {
                (_, string protocolValue) = GetHttpProtocol(url);
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork: //IPv4
                        if (Uri.TryCreate(protocolValue + address.ToString(), UriKind.RelativeOrAbsolute, out Uri newUrl))
                            return new CheckUrlModel(true, newUrl, null);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv4 address.");
                    case AddressFamily.InterNetworkV6: //IPv6
                        if (Uri.TryCreate(protocolValue + $"[{address}]", UriKind.RelativeOrAbsolute, out newUrl))
                            return new CheckUrlModel(true, newUrl, null);
                        else
                            return new CheckUrlModel(false, null, "Was unable to parse url to a IPv6 address.");
                    default:
                        return new CheckUrlModel(false, null, "Was unable to parse url to either a IPv4 or IPv6 address.");
                }
            }

            return result;
        }

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
