using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DRN_Console.Models;

namespace DRN_Console
{
    internal class GlobalFunctions
    {
		public static PostJson Json { get; set; }

		public static void MakeRequest((bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result)
		{
			if (result.result)
			{
				Console.WriteLine($"Headers:");
				foreach (var header in result.headers)
				{
					Console.WriteLine($"	Key: {header.Key}");
					Console.WriteLine($"	Value(s):");
					foreach (var value in header.Value)
					{
						Console.WriteLine($"		{value}");
					}
				}
				Console.WriteLine($"\n\n\n\nContent:\n{result.content.ReadAsStringAsync().Result}");
			}
			else
			{
				Console.WriteLine(result.errorCode);
			}
		}

		public static void ConfigFile()
		{
			string directory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			string file = Path.Combine(directory, "config.json");
			Directory.CreateDirectory(directory);

			bool configFileExits = File.Exists(file);
			if (!configFileExits)
			{
				string json = JsonConvert.SerializeObject(PostJson.GetCredExample(), Formatting.Indented, new StringEnumConverter());
				File.WriteAllText(file, json);
			}

			Json = JsonConvert.DeserializeObject<PostJson>(File.ReadAllText(file));

			if (string.IsNullOrEmpty(Json.PostUrl))
			{
				Console.WriteLine($"Set your post url in {file}");
				Environment.Exit(0);
			}
		}
	}
}
