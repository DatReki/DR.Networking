﻿using System;
using System.Linq;
using System.Collections.ObjectModel;

using static DR.Networking.Configuration;

namespace DR.Networking.Core.Extensions
{
    internal static class ObservableCollection
    {
        internal static (bool, RequestData, Base.UrlType?) FindUri(this ObservableCollection<RequestData> list, Uri target)
        {
            RequestData findPage = list.Where(e => e._url == target).FirstOrDefault();
            if (findPage == null)
            {
                RequestData findDomain = list.Where(r => r._url.AbsoluteUri == target.AbsoluteUri).FirstOrDefault();
                if (findDomain == null)
                {
                    return (false, null, null);
                }
                else
                {
                    return (true, findDomain, Base.UrlType.Domain);
                }
            }
            else
            {
                return (true, findPage, Base.UrlType.Page);
            }
        }
    }
}
