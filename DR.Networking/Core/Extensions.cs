using DR.Networking.Core.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace DR.Networking.Core
{
    internal static class Extensions
    {
        /// <summary>
        /// Round the double value up to the nearest integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static int RoundUp(this double value)
            => Convert.ToInt32(Math.Round(value, MidpointRounding.AwayFromZero));

        /// <summary>
        /// Get the name for a <see cref="Models.NamedClient"/> from <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static string GetClientName(this MemberInfo info)
        {
            string name = info.ToString();
            if (string.IsNullOrWhiteSpace(name))
                name = info.Name;

            return name;
        }

        /// <summary>
        /// Convert a <see cref="HttpMethod"/> to a <see cref="RequestTypes"/>
        /// </summary>
        /// <param name="method"></param>
        /// <returns>
        /// Returns the correlating <see cref="RequestTypes"/> type.<br />
        /// Will return <see cref="RequestTypes.Unknown"/> if none can be found.
        /// </returns>
        internal static RequestTypes GetRequestType(this HttpMethod method)
        {
            RequestTypes result;

            if (method == HttpMethod.Get)
                result = RequestTypes.Get;
            else if (method == HttpMethod.Post)
                result = RequestTypes.Post;
            else if (method == HttpMethod.Put)
                result = RequestTypes.Put;
            else if (method == HttpMethod.Patch)
                result = RequestTypes.Patch;
            else if (method == HttpMethod.Delete)
                result = RequestTypes.Delete;
            else if (method == HttpMethod.Head)
                result = RequestTypes.Head;
            else if (method == HttpMethod.Options)
                result = RequestTypes.Options;
            else if (method == HttpMethod.Trace)
                result = RequestTypes.Trace;
            else
                result = RequestTypes.Unknown;

            return result;
        }

        /// <summary>
        /// Check if the provided <see cref="HttpMethod"/> is supported by the library
        /// </summary>
        /// <param name="method"></param>
        /// <param name="requestType"></param>
        /// <returns></returns>
        internal static bool IsSupported(this HttpMethod method, out RequestTypes requestType)
        {
            RequestTypes found = method.GetRequestType();
            requestType = found;

            if (!Main.SupportedTypes.Any())
            {
                try
                {
                    Type rType = typeof(RequestTypes);
                    Type sType = typeof(Supported);

                    IEnumerable<MemberInfo> members = rType.GetMembers()
                        .Where(x => x.DeclaringType == rType && x.GetCustomAttributes(sType, false).Length > 0);

                    foreach (var member in members)
                    {
                        object attr = member.GetCustomAttributes(sType, false).FirstOrDefault();
                        if (attr != null && ((Supported)attr).IsSupported)
                        {
                            if (Enum.TryParse(member.Name, true, out RequestTypes supported))
                                Main.SupportedTypes.Add(supported);
                        }
                    }
                }
                catch
                {

                }
            }

            return Main.SupportedTypes.Any(x => x == found);
        }

        /// <summary>
        /// Clone the <see cref="HttpRequestMessage"/> so that it can be used after the request has happend.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static async Task<HttpRequestMessage?> Clone(this HttpRequestMessage request, string url = "")
        {
            HttpRequestMessage? clone = null;

            try
            {
                MemoryStream ms = new MemoryStream();
                clone = new HttpRequestMessage(request.Method, request.RequestUri);

                if (request.Content != null)
                {
                    await request.Content.CopyToAsync(ms).ConfigureAwait(false);
                    ms.Position = 0;
                    clone.Content = new StreamContent(ms);

                    // Copy the content headers
                    foreach (var h in request.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
                }

                clone.Version = request.Version;
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
                    clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

                if (!string.IsNullOrWhiteSpace(url))
                    clone.RequestUri = new Uri(url);
            }
            catch
            {

            }

            return clone;
        }
    }
}
