using System;

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
        {
            return Convert.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));
        }
    }
}
