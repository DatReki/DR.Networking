using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    }
}
