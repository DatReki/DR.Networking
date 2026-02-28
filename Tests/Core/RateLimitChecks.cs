using DR.Networking.Models;

namespace Tests.Core
{
    internal class RateLimitChecks
    {
        internal static string RemoveExistingRateLimits(List<UrlRateLimit> toRemove)
        {
            List<UrlRateLimit> needToRemove = [.. DR.Networking.RateLimiting.GetUrlRateLimits().Where(x => toRemove.Any(y => y.Uri == x.Uri))];
            if (needToRemove.Count != 0)
            {
                bool removedExisting = DR.Networking.RateLimiting.Remove(needToRemove);
                if (!removedExisting)
                    return "One or more ratelimits where already added and could not be removed";
            }

            return string.Empty;
        }
    }
}
