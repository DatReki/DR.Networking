using System.Diagnostics;

namespace DR.Networking.Models
{
    internal class UrlHistory
    {
        public UrlHistory()
        {
            Timestamp = Stopwatch.GetTimestamp();
        }

        internal string Original { get; set; } = string.Empty;
        internal UrlCheck? Checked { get; set; } = default;
        internal long Timestamp { get; set; }
    }
}
