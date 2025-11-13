using System;
using System.Reflection;

namespace DR.Networking.Core
{
    internal static class Extensions
    {
        /// <summary>
        /// Round the double value up to the nearest integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int RoundUp(this double value)
            => Convert.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Get the name for a <see cref="Models.NamedClient"/> from <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static string GetClientName(this MemberInfo info)
        {
            string name = info.ToString();
            if (string.IsNullOrWhiteSpace(name))
                name = info.Name;
            
            return name;
        }
    }
}
