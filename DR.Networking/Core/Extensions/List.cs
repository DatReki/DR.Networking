using System;
using System.Collections.Generic;

using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class List
    {
        internal static (bool, SiteSpecific) FindUri(this List<SiteSpecific> list, Uri target)
        {
            SiteSpecific findPage = list.Find(e => e._uri == target);
            if (findPage == null)
            {
                SiteSpecific findDomain = list.Find(r => r._uri.AbsoluteUri == target.AbsoluteUri);
                if (findDomain == null)
                {
                    return (false, null);
                }
                else
                {
                    return (true, findDomain);
                }
            }
            else
            {
                return (true, findPage);
            }
        }
    }
}
