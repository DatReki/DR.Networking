using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static DRN_Console.GlobalFunctions;
using DRN_Console.Models;

namespace DRN_Console.RunRequests
{
    internal class WithoutRateLimiting
    {
        internal static void Get()
        {
            Console.WriteLine("Write an url that you want to make a get request to");
            string url = Console.ReadLine();
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(url).Result;
            MakeRequest(result);
        }

        internal static void PostEncoded()
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

        internal static void PostDynamic()
        {
            ConfigFile();
            Permissions p = new() { Permission = "user", Permission_Description = "general-user-account" };

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(Json.PostUrl, p).Result;
            MakeRequest(result);
        }
    }
}
