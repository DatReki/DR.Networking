using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DRN_Console
{
	class Program
	{
		public static Model Json { get; set; }

		public partial class Permissions
		{
			public string permission { get; set; }
			public string permission_description { get; set; }
		}

		static void Main(string[] args)
		{
			List<string> requestTypes = new() { "get", "post-encoded", "post-dynamic", "get-ratelimited" };

			Console.WriteLine($"Select what type of request you want to make.\nYou can choose:");
			requestTypes.ForEach(x => Console.WriteLine(x));
			string input = Console.ReadLine().ToLower().Trim();
			if (requestTypes.Contains(input))
			{
				if (input == requestTypes.ElementAt(0))
				{
					Get();
				}
				else if (input == requestTypes.ElementAt(1))
				{
					PostEncoded();
				}
				else if (input == requestTypes.ElementAt(2))
				{
					PostDynamic();
				}
				else if (input == requestTypes.ElementAt(3))
                {
					GetRateLimited();
                }
			}
			else
			{
				Restart(args);
			}
		}

		static void Get()
        {
			Console.WriteLine("Write an url that you want to make a get request to");
			string url = Console.ReadLine();
			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(url).Result;
			MakeRequest(result);
		}

		static void PostEncoded()
        {
			ConfigFile();
			var content = new FormUrlEncodedContent(new[]
			{
						new KeyValuePair<string, string>("permission", "user"),
						new KeyValuePair<string, string>("permission_description", "general-user-account")
					});

			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(Json.PostUrl, content).Result;
			MakeRequest(result);
		}

		static void PostDynamic()
        {
			ConfigFile();
			Permissions p = new Permissions { permission = "user", permission_description = "general-user-account" };

			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(Json.PostUrl, p).Result;
			MakeRequest(result);
		}

		static void GetRateLimited()
        {
			TimeSpan globalRateLimit = TimeSpan.FromSeconds(10);
			List<DR.Networking.Configuration.SiteSpecific> rateLimitings = new List<DR.Networking.Configuration.SiteSpecific>()
			{
				new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(30), Url = "https://www.rockwellautomation.com/" },
				new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(10), Url = "https://www.rockwellautomation.com/content/dam/rockwell-automation/sites/images/logos/2019_Logo_AllenBradley_rgb.svg" },
				new DR.Networking.Configuration.SiteSpecific() { Duration = globalRateLimit, Url = "www.example.com" },
			};

			DR.Networking.Configuration configuration = new DR.Networking.Configuration(globalRateLimit, rateLimitings);

			List<string> Urls = new List<string>()
            {
				/*
				"www.google.com",
				"www.cloudflare.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.hello.com",
				"https://www.rockwellautomation.com/",
				"https://www.rockwellautomation.com/",
				"https://www.rockwellautomation.com/content/dam/rockwell-automation/sites/images/logos/2019_Logo_AllenBradley_rgb.svg",
				"https://www.rockwellautomation.com/content/dam/rockwell-automation/sites/images/logos/2019_Logo_AllenBradley_rgb.svg"
				*/
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com",
				"www.example.com"
			};
			foreach (string url in Urls)
            {
				DateTime now = DateTime.UtcNow;

				(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(url).Result;

				DateTime end = DateTime.UtcNow;
				Console.WriteLine($"Request url: {url}\nRequest status: {result.result}\nRequest start: {now}\nRequest end: {end}\nTime difference: {(end - now).TotalSeconds}\n\n\n\n");
			}
		}

		private static void Restart(string[] args)
		{
			Console.Clear();
			Console.WriteLine($"You provided an incorrect choice please try again");
			Main(args);
		}

		private static void MakeRequest((bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result)
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

		private static void ConfigFile()
		{
			string directory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			string file = Path.Combine(directory, "config.json");
			Directory.CreateDirectory(directory);

			bool configFileExits = File.Exists(file);
			if (!configFileExits)
			{
				string json = JsonConvert.SerializeObject(Model.GetCredExample(), Formatting.Indented, new StringEnumConverter());
				File.WriteAllText(file, json);
			}

			Json = JsonConvert.DeserializeObject<Model>(File.ReadAllText(file));

			if (string.IsNullOrEmpty(Json.PostUrl))
			{
				Console.WriteLine($"Set your post url in {file}");
				Environment.Exit(0);
			}
		}
	}
}
