using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Console
{
	public class Model
	{
		[DefaultValue("")]
		public string PostUrl { get; set; }

		public static Model GetCredExample()
		{
			return JsonConvert.DeserializeObject<Model>("{}", new JsonSerializerSettings()
			{
				DefaultValueHandling = DefaultValueHandling.Populate,
				Converters = new JsonConverter[] { new StringEnumConverter() }
			});
		}
	}
}
