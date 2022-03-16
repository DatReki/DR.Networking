using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace DRN_Console.Models
{
    internal class PostJson
    {
        [DefaultValue("https://ptsv2.com/")]
        public string PostUrl { get; set; }

        public static PostJson GetCredExample()
        {
            return JsonConvert.DeserializeObject<PostJson>("{}", new JsonSerializerSettings()
            {
                DefaultValueHandling = DefaultValueHandling.Populate,
                Converters = new JsonConverter[] { new StringEnumConverter() }
            });
        }
    }
}
