using DR.Networking;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System;

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
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Get(Data.s_getUrl).Result;

            if (request.result)
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
                    (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Get(url).Result;

                    if (!request.result)
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
            Dictionary<string, string> content = Global.SetHeaders(Data.Headers.Get.s_key, Data.Headers.Get.s_value);
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Get(Data.s_jsonData.PostUrl, content).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }
    }
}
