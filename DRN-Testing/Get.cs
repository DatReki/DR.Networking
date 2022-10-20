using System.Net.Http.Headers;
using System.Collections.Generic;

using DR.Networking;
using NUnit.Framework;

namespace DRN_Testing
{
    public class Get
    {
        /// <summary>
        /// Make a simgple get request
        /// </summary>
        [Test]
        public void GetRequest()
        {
            bool result = false;
            DR.Networking.Data request = Request.Get(Data.s_getUrl).Result;

            if (request.Result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Make a get request with ratelimiting
        /// </summary>
        [Test]
        public void GetRequests()
        {
            foreach (string url in Data.s_GetUrlList)
            {
                for (int i = 0; i < 2; i++)
                {
                    DR.Networking.Data request = Request.Get(url).Result;

                    if (!request.Result)
                    {
                        Assert.IsTrue(false);
                    }

                    i++;
                }
            }

            Assert.IsTrue(true);
        }

        /// <summary>
        /// Make a get request with headers
        /// </summary>
        [Test]
        public void GetWithHeadersRequest()
        {
            bool result = false;
            Dictionary<string, string> headers = Global.SetHeaders(nameof(GetWithHeadersRequest), Data.Headers.Get.s_key, Data.Headers.Get.s_value);
            DR.Networking.Data request = Request.Get(Data.s_jsonData.PostUrl, headers).Result;

            if (request.Result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        /// <summary>
        /// Make a get request with headers
        /// </summary>
        [Test]
        public void GetWithAuthHeadersRequest()
        {
            bool result = false;
            AuthenticationHeaderValue authHeader = new(Data.Headers.s_authHeader);
            DR.Networking.Data request = Request.Get(Data.s_jsonData.PostUrl, authHeader).Result;

            if (request.Result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }
    }
}
