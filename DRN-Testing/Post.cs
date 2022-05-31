using DR.Networking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Testing
{
    public class Post
    {
        [Test]
        public void PostRequestFormUrlEncodedContent()
        {
            bool result = false;
            FormUrlEncodedContent content = new(Global.SetHeaders(Data.Headers.Post.s_key, Data.Headers.Post.s_value));
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, content).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void PostRequestDynamic()
        {
            bool result = false;
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, Data.s_headersExampleData).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }
    }
}
