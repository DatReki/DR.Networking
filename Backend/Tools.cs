using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Backend
{
    public class Tools
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            Formatting = Formatting.Indented,
        };

        public static bool TryParseJson(string json, out JObject? output)
        {
            try
            {
                output = JsonConvert.DeserializeObject<JObject>(json, JsonSettings);
                if (output == null)
                    return false;

                return true;
            }
            catch
            {
                output = null;
                return false;
            }
        }

        public static bool TryParseJson<T>(string json, out T? output)
        {
            try
            {
                output = JsonConvert.DeserializeObject<T>(json, JsonSettings);
                if (output == null)
                    return false;

                return true;
            }
            catch
            {
                output = default;
                return false;
            }
        }

        public static bool TryGetFormattedJson(string input, out string output)
        {
            bool result = TryParseJson(input, out JObject? json);
            if (result && json != null)
                output = JsonConvert.SerializeObject(json, JsonSettings);
            else
                output = string.Empty;

            return result;
        }

        public static bool TryParseXml(string xml, out XDocument? xDoc)
        {
            try
            {
                xDoc = XDocument.Parse(xml);
                return true;
            }
            catch
            {
                xDoc = null;
                return false;
            }
        }

        public static bool TryGetFormattedXml(string input, out string output)
        {
            bool result = TryParseXml(input, out XDocument? xDoc);
            if (result && xDoc != null)
                output = xDoc.ToString();
            else
                output = string.Empty;

            return result;
        }

        public static bool Ipv6Available()
        {
            if (!Socket.OSSupportsIPv6)
                return false;

            IEnumerable<NetworkInterface> supportedInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(x => x.Supports(NetworkInterfaceComponent.IPv6));
            if (supportedInterfaces.Any())
                return supportedInterfaces.Any(x => x.GetIPProperties().GetIPv6Properties().Index > 0);

            return false;
        }

        public static bool TryGetIpv6(out IPAddress? ip)
        {
            ip = null;
            if (!Ipv6Available())
                return false;

            ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetworkV6 && x.ScopeId == 0);
            if (ip == null)
                return false;

            return true;
        }
    }
}
