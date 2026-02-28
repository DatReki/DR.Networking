using DR.Networking;
using DR.Networking.Core;
using DR.Networking.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

namespace ConsoleApp.Core
{
    internal class RateLimitRequests
    {
        internal static async Task<Result> SendRequest(string name, TimeSpan limit, List<string>? endpoints = null, int count = 15)
        {
            Stopwatch timer = new();
            List<KeyValuePair<Result, TimeSpan>> responses = [];

            Console.WriteLine($"Sending {count} requests");
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    timer.Start();

                string url;
                if (endpoints != null && endpoints.Count > 0)
                    url = endpoints[Generate.Main.RandomNumber(0, endpoints.Count)];
                else
                    url = "Get/RandomNumber";

                Result response = await Request.Send(new(HttpMethod.Get, url), name);
                if (i > 0)
                {
                    TimeSpan elapsed = timer.Elapsed;
                    Console.WriteLine($"Number: {i}\nDuration: {elapsed:hh\\:mm\\:ss\\.fff}\nStatus: {response.StatusCode}");
                    responses.Add(new KeyValuePair<Result, TimeSpan>(response, elapsed));
                    timer.Reset();
                }
                else
                    Console.WriteLine($"Number: {i}\nStatus: {response.StatusCode}");
            }

            StringBuilder result = new();
            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            IEnumerable<TimeSpan> tooShort = responses.Where(x => x.Value < limit).Select(x => x.Value);
            string shorter = tooShort.Any() ? "Yes" : "No";

            result.AppendLine($"Average duration '{average:hh\\:mm\\:ss\\.fff}'");
            result.AppendLine($"Any resposnes shorter than '{limit:hh\\:mm\\:ss\\.fff}'? {shorter}");
            result.AppendLine($"Shortest request '{responses.First(x => x.Value == responses.Min(y => y.Value)).Value:hh\\:mm\\:ss\\.fff}'");

            Console.WriteLine($"\nResult:\n{result}");
            List<Result> badResponses = [.. responses.Where(x => !x.Key.Success).Select(x => x.Key)];

            if (badResponses.Count != 0)
            {
                result.Clear();
                result.AppendLine("\n\nBad responses:");
                foreach (Result badResponse in badResponses)
                {
                    if (badResponse.Response == null)
                        continue;

                    string raw = await (badResponse.Response?.ToRawString() ?? Task.FromResult(string.Empty));
                    if (string.IsNullOrWhiteSpace(raw))
                        continue;

                    result.AppendLine($"\n\n{raw}\n\n");
                }
            }

            return responses.Last().Key;
        }

        internal static async Task<Result> SendParallelRequest(string name, TimeSpan limit, List<string>? endpoints = null, int count = 60)
        {
            ConcurrentBag<KeyValuePair<Result, TimeSpan>> responses = [];

            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = 2,
            };

            Console.WriteLine($"Sending {count} requests");
            await Parallel.ForEachAsync(Enumerable.Repeat(string.Empty, count), options, async (item, token) =>
            {
                Stopwatch timer = Stopwatch.StartNew();

                string url;
                if (endpoints != null && endpoints.Count > 0)
                    url = endpoints[Generate.Main.RandomNumber(0, endpoints.Count)];
                else
                    url = "Get/RandomNumber";

                Result response = await Request.Send(new(HttpMethod.Get, url), name);
                TimeSpan elapsed = timer.Elapsed;
                responses.Add(new KeyValuePair<Result, TimeSpan>(response, elapsed));
                timer.Reset();
            });

            StringBuilder result = new();
            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            IEnumerable<TimeSpan> tooShort = responses.Where(x => x.Value < limit).Select(x => x.Value);
            string shorter = tooShort.Any() ? "Yes" : "No";

            result.AppendLine($"Average duration '{average:hh\\:mm\\:ss\\.fff}'");
            result.AppendLine($"Any responses shorter than '{limit:hh\\:mm\\:ss\\.fff}'? {shorter}");
            result.AppendLine($"Shortest request '{responses.First(x => x.Value == responses.Min(y => y.Value)).Value:hh\\:mm\\:ss\\.fff}'");

            Console.WriteLine($"\nResult:\n{result}");
            List<Result> badResponses = [.. responses.Where(x => !x.Key.Success).Select(x => x.Key)];

            if (badResponses.Count != 0)
            {
                result.Clear();
                result.AppendLine("\n\nBad responses:");
                foreach (Result badResponse in badResponses)
                {
                    if (badResponse.Response == null)
                        continue;

                    string raw = await (badResponse.Response?.ToRawString() ?? Task.FromResult(string.Empty));
                    if (string.IsNullOrWhiteSpace(raw))
                        continue;

                    result.AppendLine($"\n\n{raw}\n\n");
                }
            }

            return responses.Last().Key;
        }
    }
}
