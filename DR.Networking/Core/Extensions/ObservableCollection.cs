using System;
using System.Collections.ObjectModel;
using System.Linq;
using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class ObservableCollection
    {
        /// <summary>
        /// Find an uri in the RequestData collection.
        /// </summary>
        /// <param name="list">ObservableCollection containing the requests you've previously made</param>
        /// <param name="target">The uri you want to find from the collection</param>
		/// <returns>
		///		<description>(bool, RequestData, Base.UrlType?)</description>
		///     <list type="number">
		///         <item>
		///             <description>Bool. Indicates if an item that has the target Uri was found in the collection</description>
		///         </item>
        ///         <item>
		///             <description>RequestData. If item that contains the target Uri is found return it</description>
		///         </item>
		///         <item>
		///             <description>Base.UrlType?. Indicates the type of result that is being returned. If we could only find the domain in the collection or the specific uri that was passed.</description>
		///         </item>
		///     </list>
		/// </returns>
        internal static (bool, RequestData, Base.UrlType?) FindUri(this ObservableCollection<RequestData> list, Uri target)
        {
            RequestData findPage = list
                .Where(e => e._url == target)
                .FirstOrDefault();
            if (findPage == null)
            {
                RequestData findDomain = list
                    .Where(r => r._url.AbsoluteUri == target.AbsoluteUri)
                    .FirstOrDefault();
                if (findDomain == null)
                    return (false, new RequestData(), null);
                else
                    return (true, findDomain, Base.UrlType.Domain);
            }
            else
                return (true, findPage, Base.UrlType.Page);
        }
    }
}
