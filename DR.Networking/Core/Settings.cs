namespace DR.Networking.Core
{
    internal class Settings
    {
        /// <summary>
        /// If neither HTTP or HTTPS is provided at the start of a request URL the library will use HTTPS.
        /// </summary>
        internal static bool UseHttpsByDefault { get; set; } = true;
    }
}
