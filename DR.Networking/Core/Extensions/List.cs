using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class List
    {
        public static (bool, SiteSpecific, Base.UrlType?) FindUri(this List<SiteSpecific> list, Uri target)
        {
            SiteSpecific siteSpecific = list.Where(i => i._uri == target).First();
            if (siteSpecific != null)
                return (true, siteSpecific, Base.UrlType.Page);
            else
            {
                SiteSpecific domainSpecific = list.Where(i => i._uri.AbsolutePath == target.AbsolutePath).First();
                if (domainSpecific != null)
                    return (true, siteSpecific, Base.UrlType.Domain);
                else
                    return (false, null, null);
            }
        }
    }
}
