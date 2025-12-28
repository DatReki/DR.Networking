using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        internal static double RoundUp(this double value)
            => Math.Ceiling(value);

        /// <summary>
        /// Round the double value down to the nearest integer.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static double RoundDown(this double value)
            => Math.Floor(value);

        /// <summary>
        /// Get the name for a <see cref="Models.NamedClient"/> from <see cref="MemberInfo"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static string GetClientName(this MemberInfo info)
        {
            string name = info.ToString() ?? string.Empty;
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
#if (NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER)
            else if (method == HttpMethod.Patch)
                result = RequestTypes.Patch;
#endif
            else if (method == HttpMethod.Delete)
                result = RequestTypes.Delete;
            else if (method == HttpMethod.Head)
                result = RequestTypes.Head;
            else if (method == HttpMethod.Options)
                result = RequestTypes.Options;
            else if (method == HttpMethod.Trace)
                result = RequestTypes.Trace;
#if NET8_0_OR_GREATER
            else if (method == HttpMethod.Connect)
                result = RequestTypes.Connect;
#endif
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
            RequestTypes type = method.GetRequestType();
            requestType = type;

            return Main.SupportedTypes.Any(x => x == type);
        }

        /// <summary>
        /// Check if the provided <see cref="HttpMethod"/> is supported by the library
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static bool IsSupported(this HttpMethod method)
            => IsSupported(method, out _);

        /// <summary>
        /// Clone the <see cref="HttpRequestMessage"/> so that it can be used after the request has happend.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        internal static async Task<HttpRequestMessage?> Clone(this HttpRequestMessage request, Uri? url = null)
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
                    foreach (KeyValuePair<string, IEnumerable<string>> h in request.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
                }

                clone.Version = request.Version;
                foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
                    clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

                if (url != null)
                    clone.RequestUri = url;
            }
            catch
            {

            }

            return clone;
        }

        /// <summary>
        /// Add a range of items to a <see cref="ObservableCollection{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="items"></param>
        internal static void AddRange<T>(this ObservableCollection<T> collection, List<T> items)
        {
            for (int i = 0; i < items.Count; i++)
                collection.Add(items[i]);
        }

        /// <summary>
        /// Remove any double '/' characters from a string. <br />
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        internal static Uri RemoveDoubleSlashes(this Uri uri)
            => new(CompiledRegex.DoubleSlashes.Replace(uri.ToString(), @"/"));

        /// <summary>
        /// Inserts a list of elements into the <see cref="ObservableCollection{T}"/> starting at the specified index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <param name="items"></param>
        internal static void InsertRange<T>(this ObservableCollection<T> collection, int index, List<T> items)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    collection.Insert(index, items[i]);
                    index++;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
