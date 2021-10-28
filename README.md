# DR.Networking

<a href="https://github.com/DatReki/DR.Networking/actions/workflows/dotnet.yml">
    <img src="https://github.com/DatReki/DR.Networking/actions/workflows/dotnet.yml/badge.svg" />
</a>
<a href="https://www.paypal.com/donate?hosted_button_id=WRETYRRSJ4T2L">
    <img src="https://img.shields.io/badge/Donate-PayPal-green.svg?style=flat-square">
</a>

A small networking library to make get & post requests

## Usage
Make a get request
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

Or make a post request using dynamic data
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
1. <strong>result</strong>: Boolean which indicates if the request passed or failed.
2. <strong>errorCode</strong>: A string containing the error message in case the request fails.
3. <strong>content</strong>: HttpContent containing the content returned by the request.
4. <strong>headers</strong>: HttpResponseHeaders containing the headers returned by the request.
