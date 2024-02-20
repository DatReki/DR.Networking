using DR.Networking.Core;
using DR.Networking.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace DR.Networking
{
    public class Configuration
    {
        /// <summary>
        /// Pass a custom configuration for the library to use.
        /// </summary>
        /// <param name="duration">
        /// A global ratelimit applied to all request. <br /> 
        /// This value will be overwritten if the url called is in the <see cref="UrlSpecificRateLimiting"/> list.
        /// </param>
        /// <param name="urlSpecific">A list of url/domain specific rate limit settings.</param>
        /// <param name="client">Pass your own HttpClient for the library to use.</param>
        /// <param name="defaultHttp">
        /// By default if neither HTTP or HTTPS is provided at the start of the request url the library will use HTTPS. <br />
        /// If you pass true to this parameter the library will add HTTP by default to the request url.
        /// </param>
        public Configuration(TimeSpan? duration, List<UrlSpecificRateLimiting> urlSpecific, HttpClient? client, bool? defaultHttp)
        {
            if (duration != null) { }

            if (urlSpecific != null) { }

            if (client != null)
                Main.Client = client;

            if (defaultHttp != null)
                Settings.UseHttpsByDefault = defaultHttp != true;
        }
    }
}
