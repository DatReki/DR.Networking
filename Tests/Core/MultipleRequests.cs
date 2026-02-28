using DR.Networking;
using DR.Networking.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;

namespace Tests.Core
{
    internal class MultipleRequests
    {
        internal static async Task<(List<KeyValuePair<Result, TimeSpan>> Responses, TimeSpan Average, TimeSpan Shortest)> SendLoopedRequest(string name, List<string>? requestUris = null, int count = 15)
        {
            Stopwatch timer = new();
            List<KeyValuePair<Result, TimeSpan>> responses = [];

            for (int i = 0; i < count; i++)
            {
                // First request won't been rate limited so we ignore it for more accurate results
                if (i > 1)
                    timer.Start();

                Result response;
                if (requestUris != null)
                {
                    int index = RandomNumberGenerator.GetInt32(0, requestUris.Count);
                    response = await Request.Send(new(HttpMethod.Get, requestUris[index]), name);
                }
                else
                    response = await Request.Send(new(HttpMethod.Get, "Get/RandomNumber"), name);

                // First request won't been rate limited so we ignore it for more accurate results
                if (i > 1)
                {
                    responses.Add(new KeyValuePair<Result, TimeSpan>(response, timer.Elapsed));
                    timer.Reset();
                }
            }

            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            TimeSpan shortest = responses.Min(x => x.Value);

            return (responses, average, shortest);
        }

        internal static async Task<(List<KeyValuePair<Result, TimeSpan>> Responses, TimeSpan Average, TimeSpan Shortest)> SendParallelRequests(string name, List<string>? requestUris = null, int count = 15)
        {
            ConcurrentBag<KeyValuePair<Result, TimeSpan>> responses = [];
            ParallelOptions options = new()
            {
                MaxDegreeOfParallelism = 2,
            };

            async Task<Result> SendRequest()
            {
                Result response;
                if (requestUris != null)
                {
                    int index = RandomNumberGenerator.GetInt32(0, requestUris.Count);
                    response = await Request.Send(new(HttpMethod.Get, requestUris[index]), name);
                }
                else
                    response = await Request.Send(new(HttpMethod.Get, "Get/RandomNumber"), name);

                return response;
            }

            // First request won't been rate limited so we ignore it for more accurate results
            await SendRequest();

            await Parallel.ForEachAsync(Enumerable.Repeat(string.Empty, count), options, async (item, token) =>
            {

                Stopwatch timer = Stopwatch.StartNew();
                Result response = await SendRequest();
                TimeSpan elapsed = timer.Elapsed;

                responses.Add(new KeyValuePair<Result, TimeSpan>(response, elapsed));
                timer.Reset();
            });

            TimeSpan average = TimeSpan.FromMilliseconds(responses.Average(x => x.Value.TotalMilliseconds));
            TimeSpan shortest = responses.Min(x => x.Value);

            return (responses.ToList(), average, shortest);
        }
    }
}
