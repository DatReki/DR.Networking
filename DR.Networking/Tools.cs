using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OrgStopwatch = System.Diagnostics.Stopwatch;

namespace DR.Networking
{
    internal class Tools
    {
        /// <summary>
        /// Check if the <see cref="IEnumerable{string}"/> contains any values which start with the 'original' <see cref="string"/>. <br />
        /// If any values can be found the 'found' <see cref="List{string}"/> parameter will return a sorted list descending based on the length of the found <see cref="string"/>s.
        /// </summary>
        /// <param name="original"></param>
        /// <param name="compare"></param>
        /// <param name="found"></param>
        /// <returns></returns>
        internal static bool TryGetStartsWith(Uri url, ObservableCollection<UrlRateLimit> ratelimits, out List<UrlRateLimit> found)
        {
            bool result = false;
            found = new List<UrlRateLimit>();

            IEnumerable<UrlRateLimit> compare = ratelimits.Where(x => x.Uri.Host == url.Host);
            int count = compare.Count();
            if (count == 0)
                return result;

            string original = url.ToString();
            for (int i = 0; i < count; i++)
            {
                UrlRateLimit check = compare.ElementAt(i);
                if (original.StartsWith(check.UrlString))
                    found.Add(check);
            }

            if (found.Any())
            {
                found = found.OrderByDescending(x => x.UrlString.Length).ToList();
                result = true;
            }

            return result;
        }

        internal partial class Stopwatch
        {
            internal static readonly double TickFrequency = (double)TimeSpan.TicksPerSecond / OrgStopwatch.Frequency;

            /// <summary>Gets the elapsed time since the <paramref name="startingTimestamp"/> value retrieved.</summary>
            /// <param name="startingTimestamp">The timestamp marking the beginning of the time period.</param>
            /// <returns>A <see cref="TimeSpan"/> for the elapsed time between the starting timestamp and the time of this call.</returns>
            public static TimeSpan GetElapsedTime(long startingTimestamp) =>
                GetElapsedTime(startingTimestamp, OrgStopwatch.GetTimestamp());

            /// <summary>Gets the elapsed time between two timestamps retrieved.</summary>
            /// <param name="startingTimestamp">The timestamp marking the beginning of the time period.</param>
            /// <param name="endingTimestamp">The timestamp marking the end of the time period.</param>
            /// <returns>A <see cref="TimeSpan"/> for the elapsed time between the starting and ending timestamps.</returns>
            public static TimeSpan GetElapsedTime(long startingTimestamp, long endingTimestamp) =>
                new TimeSpan((long)((endingTimestamp - startingTimestamp) * TickFrequency));
        }
    }
}
