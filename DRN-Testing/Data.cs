using DR.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Testing
{
    internal class Data
    {
        internal static DRN_Core.Models.Data s_jsonData { get; set; }

        internal static string s_getUrl = "google.com";

        internal static List<string> s_GetUrlList = new List<string>()
        {
            "https://example.com/",
            "https://example.org/",
            "https://example.net/",
            "https://example.edu/",
            "https://example.nl/by-sidn/",
        };

        internal static TimeSpan s_globalRateLimit = TimeSpan.FromSeconds(10);

        internal static List<Configuration.SiteSpecific> s_rateLimits = GetRateLimits();

        internal partial class Headers
        {
            private static string s_start = "_Unit_Test_";
            private static string s_key_End = "_Header";
            private static string s_value_End = "_Value";

            internal partial class Get
            {
                internal static string s_key
                {
                    get
                    {

                        return $"{s_start}Get{s_key_End}";
                    }
                }

                internal static string s_value
                {
                    get
                    {

                        return $"{s_start}Get{s_value_End}";
                    }
                }
            }

            internal partial class Post
            {
                internal static string s_key
                {
                    get
                    {

                        return $"{s_start}Post{s_key_End}";
                    }
                }

                internal static string s_value
                {
                    get
                    {
                        return $"{s_start}Post{s_value_End}";
                    }
                }
            }
        }

        internal partial class HeadersExample
        {
            internal string example_name { get; set; }
            internal string example_description { get; set; }
        }

        internal static HeadersExample s_headersExampleData
        {
            get
            {
                return new HeadersExample { example_name = "user", example_description = "general-user-account" }; ;
            }
        }

        internal static List<Configuration.SiteSpecific> GetRateLimits()
        {
            List<Configuration.SiteSpecific> result = new();
            int count = 5;

            foreach (string url in s_GetUrlList)
            {
                result.Add(new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(count), Url = url });
                count += 5;
            }

            return result;
        }
    }
}
