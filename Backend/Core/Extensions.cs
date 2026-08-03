using DR.Networking.Models;

namespace Backend.Core
{
    public static class Extensions
    {
        public static string GetBaseAddress(this NamedClient? client)
        {
            string result = string.Empty;
            if (client?.Client?.BaseAddress != null)
            {
                result = client?.Client?.BaseAddress.ToString() ?? string.Empty;
                if (result.EndsWith('/'))
                    result = result.TrimEnd('/');
            }

            return result;
        }
    }
}
