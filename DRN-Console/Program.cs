using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DRN_Console
{
	class Program
	{
		public static List<string> RequestTypes = new() { "get", "get-headers", "post-encoded", "post-dynamic" };

		public static string[] Args;

		enum RequestType
        {
			RateLimiting,
			Normal
        }

		static void Main(string[] args)
		{
			Args = args;
			Global.ColoredConsole("Do you want to make requests with or without ratelimiting? (with/without)");
			switch (Global.UserInput())
            {
				case "with":
					SelectRequest(RequestType.RateLimiting);
					break;
				case "without":
					SelectRequest(RequestType.Normal);
					break;
				default:
					Restart();
					break;
            }
		}

		static void SelectRequest(RequestType type)
        {
			switch (type)
            {
				case RequestType.RateLimiting:
					RunRequests.WithRateLimiting.Get();
					break;
				case RequestType.Normal:
					Global.ColoredConsole("Select what type of request you want to make.\nYou can choose:");
					RequestTypes.ForEach(x => Global.ColoredConsole($"	{x}"));
					string input = Global.UserInput();
					if (RequestTypes.Contains(input))
					{
						if (input == RequestTypes.ElementAt(0))
						{
							RunRequests.WithoutRateLimiting.Get();
						}
						else if (input == RequestTypes.ElementAt(1))
                        {
							RunRequests.WithoutRateLimiting.GetWithHeaders();
						}
						else if (input == RequestTypes.ElementAt(2))
						{
							RunRequests.WithoutRateLimiting.PostEncoded();
						}
						else if (input == RequestTypes.ElementAt(3))
						{
							RunRequests.WithoutRateLimiting.PostDynamic();
						}
					}
					else
					{
						Restart();
					}
					break;
			}
		}

		public static void Restart()
		{
			Console.Clear();
			Global.ColoredConsole($"You provided an incorrect choice please try again");
			Main(Args);
		}
	}
}
