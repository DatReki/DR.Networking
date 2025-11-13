using DR.Networking.Core;
using DR.Networking.Models;
using System.Linq;
using System.Net.Http;

namespace DR.Networking
{
    public class Clients
    {
        /// <summary>
        /// Create a <see cref="NamedClient"/> by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static NamedClient CreateClient(string name, HttpClient client)
            => new NamedClient(name, client);

        /// <summary>
        /// Create a <see cref="NamedClient"/> by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static NamedClient CreateClient<T>(HttpClient client)
            => new NamedClient(typeof(T).GetClientName(), client);

        /// <summary>
        /// Get a <see cref="NamedClient"/> you have added to the library.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamedClient? GetClient(string name)
            => Settings.NamedClients.FirstOrDefault(x => x.Name == name);

        /// <summary>
        /// Get a <see cref="NamedClient"/> you have added to the library.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NamedClient? GetClient<T>()
            => Settings.NamedClients.FirstOrDefault(x => x.Name == typeof(T).Name);

        /// <summary>
        /// Try and get a <see cref="NamedClient"/> you have added to the library.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="false"/> if no client can be found with the provided name. Otherwise <see cref="true"/>.</returns>
        public static bool TryGetClient(string name, out NamedClient client)
        {
            bool result = false;
            NamedClient? found = null;

            try
            {
                found = GetClient(name);
            }
            catch
            {

            }

            client = found ?? new NamedClient();
            return result;
        }

        /// <summary>
        /// Try and get a <see cref="NamedClient"/> you have added to the library.
        /// </summary>
        /// <param name="name"></param>
        /// <returns><see cref="false"/> if no client can be found with the provided type. Otherwise <see cref="true"/>.</returns>
        public static bool TryGetClient<T>(out NamedClient client)
        {
            bool result = false;
            NamedClient? found = null;

            try
            {
                found = GetClient(typeof(T).GetClientName());
            }
            catch
            {

            }

            client = found ?? new NamedClient();
            return result;
        }

        /// <summary>
        /// Try to add a <see cref="NamedClient"/> to the library.
        /// </summary>
        /// <param name="client"></param>
        /// <returns><see cref="true"/> if the client can be added. <see cref="false"/> if a client with the same name has already been added to the library</returns>
        public static bool Add(NamedClient client)
        {
            bool result = false;

            if (!Settings.NamedClients.Any(x => x.Name == client.Name))
            {
                Settings.NamedClients.Add(client);
                result = true;
            }

            return result;
        }
    }
}
