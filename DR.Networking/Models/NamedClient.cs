using System.Net.Http;

namespace DR.Networking.Models
{
    public class NamedClient
    {
        public NamedClient() { }

        /// <summary>
        /// Create named HttpClient with string.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="client"></param>
        public NamedClient(string name, HttpClient client)
        {
            Name = name;
            Client = client;
        }

        /// <summary>
        /// Name associated with this HttpClient.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The HttpClient.
        /// </summary>
        public HttpClient? Client { get; set; }
    }
}
