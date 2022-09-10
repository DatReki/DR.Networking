# DR.Networking
A small networking library to make get & post requests

## Usage
Make a get request with rate limiting
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        #region config
        //Assign a global rate limit time. When you make a request to the same URL twice the second time this rate limit will be applied.
        //Unless you manually specify a rate limit for that page/domain in List<Configuration.SiteSpecific>.
        TimeSpan globalRateLimit = TimeSpan.FromSeconds(10);

        //Create a list of page/domain specific rate limits. Page specific rate limits will be prioritized followed by domain specific rate limits.
        //If you pass a url which isn't contained in either global rate limit will be used.
        List<Configuration.SiteSpecific> rateLimitings = new()
        {
            new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(5), Url = "https://example.com/" },
            new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(10), Url = "https://example.org/" },
            new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(15), Url = "https://example.net/" },
            new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(20), Url = "https://example.edu/" },
            new Configuration.SiteSpecific() { Duration = TimeSpan.FromSeconds(25), Url = "https://example.nl/by-sidn/" },
        };

        //Pass the rate limit to the library (you will only need to set this once).
        _ = new Configuration(globalRateLimit, rateLimitings);
        #endregion

        //Below is just for example. Run these requests however you want.
        List<string> Urls = new()
        {
            "https://example.com/",
            "https://example.com/",
            "https://example.org/",
            "https://example.org/",
            "https://example.net/",
            "https://example.net/",
            "https://example.edu/",
            "https://example.edu/",
            "https://example.nl/by-sidn/",
            "https://example.nl/by-sidn/",
        };

        foreach (string url in Urls)
        {
            var request = Request.Get(url).Result;
            if (request.result)
            {
                Console.WriteLine(request.content.ReadAsStringAsync().Result);
            }
            else
            {
                //Handle errors/issues
                Console.WriteLine(request.errorCode);
            }
        }
    }
}
```
Everything in the config region only really needs to be configured/setup once. You can do this in a startup/program file for example. So once you have it setup you don't need to include it in individual requests. Once you do have it configured it will apply ratelimiting to every request if 'globalRateLimit' is specified. Or on every request using a url specified in your 'rateLimitings' list.

Make a get request without rate limiting
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        var request = Request.Get(url).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Make a get request with headers
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        var headers = new Dictionary<string, string>
        {
            { "permission", "user" },
            { "permission_description", "general-user-account" }
        };
	
        var request = Request.Get(url, headers).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Make a get request with auth headers
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("password");
	
        var request = Request.Get(url, authHeader).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Make a post request (using FormUrlEncodedContent)
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("permission", "user"),
            new KeyValuePair<string, string>("permission_description", "general-user-account")
        });

        var request = Request.Post(url, content).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Make a post request with headers
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("permission", "user"),
            new KeyValuePair<string, string>("permission_description", "general-user-account")
        });

        var headers = new Dictionary<string, string>
        {
            { "permission", "user" },
            { "permission_description", "general-user-account" }
        };

        var request = Request.Post(url, content, headers).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Make a post request with auth headers
```cs
using DR.Networking;

class Program
{
    static void Main(string[] args)
    {
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("permission", "user"),
            new KeyValuePair<string, string>("permission_description", "general-user-account")
        });

        AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("password");

        var request = Request.Post(url, content, authHeader).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

Or make a post request using dynamic data (also both type of post requests support headers)
```cs
using DR.Networking;

class Program
{
    public partial class Permissions
    {
        public string permission { get; set; }
        public string permission_description { get; set; }
    }

    static void Main(string[] args)
    {
        Permissions content = new Permissions { permission = "user", permission_description = "general-user-account" };

        var request = Request.Post(url, content).Result;
        if (request.result)
        {
            Console.WriteLine(request.content.ReadAsStringAsync().Result);
        }
        else
        {
            //Handle errors/issues
            Console.WriteLine(request.errorCode);
        }
    }
}
```

### Return values
1. **result**: Boolean which indicates if the request passed or failed.
2. **errorCode**: A string containing the error message in case the request fails.
3. **content**: HttpContent containing the content returned by the request.
4. **headers**: HttpResponseHeaders containing the headers returned by the request.
