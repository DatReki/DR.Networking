using System;
using System.Collections.Generic;

using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class List
    {
        /// <summary>
        /// Find an uri in the SiteSpecific list.
        /// </summary>
        /// <param name="list">List containing the sites you want to ratelimit</param>
        /// <param name="target">The uri you want to find from the list</param>
		/// <returns>
		///		<description>(bool, SiteSpecific)</description>
		///     <list type="number">
		///         <item>
		///             <description>Bool. Indicates if an item that has the target Uri was found in the list</description>
		///         </item>
		///         <item>
		///             <description>SiteSpecific. If item that contains target Uri is found return it</description>
		///         </item>
		///     </list>
		/// </returns>
        internal static (bool, SiteSpecific) FindUri(this List<SiteSpecific> list, Uri target)
        {
            SiteSpecific findPage = list.Find(e => e._uri == target);
            if (findPage == null)
            {
                SiteSpecific findDomain = list.Find(r => r._uri.AbsoluteUri == target.AbsoluteUri);
                if (findDomain == null)
                    return (false, new SiteSpecific());
                else
                    return (true, findDomain);
            }
            else
                return (true, findPage);
        }
    }
}
