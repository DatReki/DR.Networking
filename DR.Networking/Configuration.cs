using DR.Networking.Core;
using DR.Networking.Models;
using System;
using System.Linq;

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
            RateLimiter.ListenToChanges();

            if (options.GlobaRateLimit != null)
                RateLimiting.UpdateGlobal((TimeSpan)options.GlobaRateLimit);

            if (options.UrlRateLimits != null)
                RateLimiting.Add(options.UrlRateLimits);

            if (options.BaseClient != null)
                Settings.Client = options.BaseClient;

            if (options.NamedClients != null)
            {
                if (options.NamedClients.GroupBy(x => x.Name).Any(x => x.Count() > 1))
                    throw new Exception($"You can't add multiple {nameof(NamedClient)}'s with the same name!");

                Settings.NamedClients.AddRange(options.NamedClients.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToList());
            }

            Settings.CloneRequestMessage = options.CloneRequestMessage;
            Settings.UseHttpsByDefault = options.UseHttpsByDefault;
            Settings.ValidateUrl = options.ValidateUrl;
        }
    }
}
