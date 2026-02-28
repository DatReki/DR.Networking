using DR.Networking.Core;
using DR.Networking.Models;
using System;
using System.Linq;
using System.Net.Http;

namespace DR.Networking
{
    public class Configuration
    {
        /// <summary>
        /// Initalize the library. You must call this class before utilizing the library.
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="Exception"></exception>
        public Configuration(ConfigurationOptions options)
        {
            Settings.ListenToChanges();
            History.ListenToChanges();

            SetConfiguration(options, out string error);
            if (!string.IsNullOrWhiteSpace(error))
                throw new Exception(error);
        }

        /// <summary>
        /// Updates the the <see cref="ConfigurationOptions"/> used by the library.
        /// </summary>
        /// <param name="options"></param>
        public static bool UpdateConfiguration(ConfigurationOptions options, out string error)
        {
            SetConfiguration(options, out error);
            if (string.IsNullOrWhiteSpace(error))
                return true;

            return false;
        }


        private static void SetConfiguration(ConfigurationOptions options, out string error)
        {
            error = string.Empty;
            RateLimiting.UpdateGlobal(options.GlobaRateLimit);
            RateLimiting.UpdateRateLimitTimeout(options.RateLimitTimeout);

            if (options.UrlRateLimits != null)
                RateLimiting.Add(options.UrlRateLimits);

            if (options.BaseClient == null)
            {
                Settings.Client = new(new StandardSocketsHttpHandler()
                {
                    PooledConnectionIdleTimeout = TimeSpan.FromMinutes(1),
                    PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                });
            }
            else
                Settings.Client = options.BaseClient;

            if (options.NamedClients != null)
            {
                if (options.NamedClients.GroupBy(x => x.Name).Any(x => x.Count() > 1))
                {
                    error = $"You can't add multiple {nameof(NamedClient)}'s with the same name!";
                    return;
                }

                Settings.NamedClients.AddRange([.. options.NamedClients.Where(x => !string.IsNullOrWhiteSpace(x.Name))]);
            }

            Settings.CloneRequestMessage = options.CloneRequestMessage;
            Settings.UseHttpsByDefault = options.UseHttpsByDefault;
            Settings.ValidateUrl = options.ValidateUrl;
        }
    }
}
