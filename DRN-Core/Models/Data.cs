using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;

namespace DRN_Core.Models
{
    public class Data
    {
		[DefaultValue("https://ptsv2.com/")]
		public string PostUrl { get; set; }

		public static Data GetJsonExample()
		{
			return JsonConvert.DeserializeObject<Data>("{}", new JsonSerializerSettings()
			{
				DefaultValueHandling = DefaultValueHandling.Populate,
				Converters = new JsonConverter[] { new StringEnumConverter() }
			});
		}
	}
}
