using System.Text.RegularExpressions;

namespace DR.Networking.Core
{
    internal class CompiledRegex
    {
        /// <summary>
        /// Match all the double slashes in a url.
        /// </summary>
        internal static readonly Regex DoubleSlashes = new Regex(@"(?<!(http:|https:))//", RegexOptions.Compiled);

        /// <summary>
        /// Get a ip address from a string.
        /// </summary>
        internal static readonly Regex Ip = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", RegexOptions.Compiled);
    }
}
