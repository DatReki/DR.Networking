using Microsoft.Extensions.Primitives;

namespace Api.Core
{
    internal static class Extensions
    {
        /// <summary>
        /// Try and get a single <see cref="string"/> value from a specific header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryGetHeaderValue(this IHeaderDictionary headers, string key, out string value)
        {
            bool result = false;
            value = string.Empty;

            if (headers.TryGetHeaderValues(key, out StringValues values))
            {
                if (values.Count != 0)
                {
                    value = values.First() ?? string.Empty;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Try and get <see cref="StringValues"/> from a specific header
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static bool TryGetHeaderValues(this IHeaderDictionary headers, string key, out StringValues values)
        {
            values = new StringValues();
            bool result = false;

            if (!headers.ContainsKey(key))
                return result;

            if (headers.TryGetValue(key, out values))
                result = true;

            return result;
        }
    }
}
