using DR.Networking.Core;
using DR.Networking.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace DR.Networking
{
    public class Clients
    {
        /// <summary>
        /// Create a <see cref="NamedClient"/> by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static NamedClient CreateClient(string name, HttpClient client)
            => new(name, client);

        /// <summary>
        /// Create a <see cref="NamedClient"/> by type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        public static NamedClient CreateClient<T>(HttpClient client)
            => new(typeof(T).GetClientName(), client);

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
        /// Get a list of <see cref="NamedClient"/>s that have been added to the library.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetClientNames()
            => [.. Settings.NamedClients.Select(x => x.Name)];

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
        /// Try to add multiple <see cref="NamedClient"/>'s to the library.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns><see cref="true"/> if any client was added otherwise <see cref="false"/>.</returns>
        public static bool Add(List<NamedClient> clients)
        {
            bool result = false;
            foreach (NamedClient client in clients)
            {
                bool added = Add(client);
                if (!result && added)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Try to add a single <see cref="NamedClient"/> to the library.
        /// </summary>
        /// <param name="client"></param>
        /// <returns><see cref="true"/> if the client was added otherwise <see cref="false"/>.</returns>
        public static bool Add(NamedClient client)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(client.Name))
                return result;

            if (!Settings.NamedClients.Any(x => x.Name == client.Name))
            {
                Settings.NamedClients.Add(client);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Try to remove multiple <see cref="NamedClient"/>'s from the library.
        /// </summary>
        /// <param name="clients"></param>
        /// <returns><see cref="true"/> if any client has been removed otherwise <see cref="false"/>.</returns>
        public static bool Remove(List<NamedClient> clients)
        {
            bool result = false;
            foreach (NamedClient client in clients)
            {
                bool added = Remove(client);
                if (!result && added)
                    result = true;
            }

            return result;
        }

        /// <summary>
        /// Try to remove a <see cref="NamedClient"/> from the library.
        /// </summary>
        /// <param name="client"></param>
        /// <returns><see cref="true"/> if the client has been removed otherwise <see cref="false"/>.</returns>
        public static bool Remove(NamedClient client)
        {
            bool result = false;
            if (string.IsNullOrWhiteSpace(client.Name))
                return result;

            NamedClient? found = Settings.NamedClients.FirstOrDefault(x => x.Name == client.Name);
            if (found != null)
            {
                Settings.NamedClients.Remove(found);
                result = true;
            }

            return result;
        }
    }
}
