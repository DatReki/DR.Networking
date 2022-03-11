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
		public static List<string> RequestTypes = new() { "get", "post-encoded", "post-dynamic" };

		public static string[] Args;

		enum RequestType
        {
			RateLimiting,
			Normal
        }

		static void Main(string[] args)
		{
			Args = args;
			Console.WriteLine("Do you want to make requests with or without ratelimiting? (with/without)");
			switch (Console.ReadLine())
            {
				case "with":
					SelectRequest(args, RequestType.RateLimiting);
					break;
				case "without":
					SelectRequest(args, RequestType.Normal);
					break;
				default:
					Restart();
					break;
            }
		}

		static void SelectRequest(string[] args, RequestType type)
        {
			Console.WriteLine($"Select what type of request you want to make.\nYou can choose:");
			RequestTypes.ForEach(x => Console.WriteLine(x));
			string input = Console.ReadLine().ToLower().Trim();
			if (RequestTypes.Contains(input))
			{
				if (input == RequestTypes.ElementAt(0))
				{
					switch (type)
                    {
						case RequestType.Normal:
							RunRequests.WithoutRateLimiting.Get();
							break;
						case RequestType.RateLimiting:
							RunRequests.WithRateLimiting.Get();
							break;
                    }
				}
				else if (input == RequestTypes.ElementAt(1))
				{
					switch (type)
					{
						case RequestType.Normal:
							RunRequests.WithoutRateLimiting.PostEncoded();
							break;
						case RequestType.RateLimiting:
							RunRequests.WithRateLimiting.PostEncoded();
							break;
					}			
				}
				else if (input == RequestTypes.ElementAt(2))
				{
					switch (type)
					{
						case RequestType.Normal:
							RunRequests.WithoutRateLimiting.PostDynamic();
							break;
						case RequestType.RateLimiting:
							RunRequests.WithRateLimiting.PostDynamic();
							break;
					}		
				}
			}
			else
			{
				Restart();
			}
		}

		static void RequestsWithRateLimiting(string[] args)
        {


			/*
			TimeSpan globalRateLimit = TimeSpan.MinValue;
			List<DR.Networking.Configuration.SiteSpecific> siteRateLimits = new() { };

			Console.WriteLine("Do you want a global rate limit? (yes/no)");
			switch (Console.ReadLine())
            {
				case "yes":
					Console.WriteLine("How many seconds do you want this rate limit to be?");
					if (int.TryParse(Console.ReadLine(), out int result))
						globalRateLimit = TimeSpan.FromSeconds(result);
					else
						Restart(args);
					break;
				case "no":
					break;
				default:
					Restart(args);
					break;
			}

			Console.WriteLine("Do you want to add site/url specific rate limits?");
			switch (Console.ReadLine())
			{
				case "yes":

					Console.WriteLine("Enter 'done' when you're finished.");

					bool inProgress = true;
					string url = null;
					int counter = 0;

					while (inProgress)
					{
						if (counter % 2 == 0)
						{
							Console.WriteLine("Please enter the amount of seconds you want the ratelimit to last");
							if (int.TryParse(Console.ReadLine(), out int result))
								siteRateLimits.Add(
									new DR.Networking.Configuration.SiteSpecific() { Url = url, Duration = TimeSpan.FromSeconds(result) });
							else
								Restart(args);
						}
						else
                        {
							Console.WriteLine("Please enter the url for the site/page that you want to add a rate limit for");
							string answer = Console.ReadLine();
							switch (answer)
                            {
								case "done":
									inProgress = false;
									break;
								default:
									url = answer;
									break;
                            }
                        }
						counter++;
					}
					break;
				case "no":
					break;
				default:
					Restart(args);
					break;
			}


			if ()
			*/
		}

		static void GetRateLimited()
        {
			TimeSpan globalRateLimit = TimeSpan.FromSeconds(10);
			List<DR.Networking.Configuration.SiteSpecific> rateLimitings = new()
			{
				new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(30), Url = "https://www.rockwellautomation.com/" },
				new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(10), Url = "https://www.rockwellautomation.com/content/dam/rockwell-automation/sites/images/logos/2019_Logo_AllenBradley_rgb.svg" },
				new DR.Networking.Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(10), Url = "www.example.com" },
			};

			DR.Networking.Configuration configuration = new(globalRateLimit, rateLimitings);

			List<string> Urls = new()
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
			DateTime start = DateTime.UtcNow;

			(bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = new();
			DateTime now;
			DateTime end;

			foreach (string url in Urls)
            {
				now = DateTime.UtcNow;

				result = DR.Networking.Request.Get(url).Result;

				end = DateTime.UtcNow;
				Console.WriteLine($"Request url: {url}\nRequest status: {result.result}\nRequest start: {now}\nRequest end: {end}\nTime difference: {(end - now).TotalSeconds}\n\n\n\n");
			}
			DateTime finish = DateTime.UtcNow;

			Console.WriteLine($"Request done. Expected around 90 seconds. Result: {(finish - start).TotalSeconds}s\n\n\n\n");
			System.Threading.Thread.Sleep(20000);

			now = DateTime.UtcNow;
			result = DR.Networking.Request.Get("www.google.com").Result;
			end = DateTime.UtcNow;

			Console.WriteLine($"Request url: www.google.com\nRequest status: {result.result}\nRequest start: {now}\nRequest end: {end}\nTime difference: {(end - now).TotalSeconds}\n\n\n\n");
		}

		public static void Restart()
		{
			Console.Clear();
			Console.WriteLine($"You provided an incorrect choice please try again");
			Main(Args);
		}
	}
}
