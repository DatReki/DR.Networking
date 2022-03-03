using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class ObservableCollection
    {
        public static (bool, RequestData, Base.UrlType?) FindUri(this ObservableCollection<RequestData> list, Uri target)
        {
            RequestData siteSpecific = list.Where(i => i._url == target).First();
            if (siteSpecific != null)
                return (true, siteSpecific, Base.UrlType.Page);
            else
            {
                RequestData domainSpecific = list.Where(i => i._url.AbsolutePath == target.AbsolutePath).First();
                if (domainSpecific != null)
                    return (true, siteSpecific, Base.UrlType.Domain);
                else
                    return (false, null, null);
            }
        }
    }
}
