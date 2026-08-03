using DR.Networking;
using DR.Networking.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace Tests.Core
{
    internal class MultipleRequests
    {
        internal static async Task<List<Result>> SendLoopedRequest(string name, List<string>? requestUris = null, int count = 15, int minDelay = 200)
        {
            List<Result> responses = [];

            for (int i = 0; i < count; i++)
            {
                Result response;
                if (requestUris != null)
                {
                    int index = RandomNumberGenerator.GetInt32(0, requestUris.Count);
                    response = await Request.Send(new(HttpMethod.Get, requestUris[index]), name);
                }
                else
                    response = await Request.Send(new(HttpMethod.Get, "Get/RandomNumber"), name);

                responses.Add(response);
            }

            return responses;
        }

        internal static async Task<List<Result>> SendParallelRequests(string name, List<string>? requestUris = null, int count = 15)
        {
            ConcurrentBag<Result> responses = [];
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

            await Parallel.ForEachAsync(Enumerable.Repeat(string.Empty, count), options, async (item, token) =>
            {
                responses.Add(await SendRequest());
            });

            return [.. responses];
        }
    }
}
