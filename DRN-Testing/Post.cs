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
            FormUrlEncodedContent content = Global.SetContent(nameof(PostRequestFormUrlEncodedContent));
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, content).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void PostRequestFormUrlEncodedContentWithHeaders()
        {
            bool result = false;

            FormUrlEncodedContent content = Global.SetContent(nameof(PostRequestFormUrlEncodedContentWithHeaders));
            Dictionary<string, string> headers = Global.SetHeaders(nameof(PostRequestFormUrlEncodedContentWithHeaders), Data.Headers.Post.s_key, Data.Headers.Post.s_value);

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, content, headers).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void PostRequestFormUrlEncodedContentWithAuthHeaders()
        {
            bool result = false;
            FormUrlEncodedContent content = Global.SetContent(nameof(PostRequestFormUrlEncodedContentWithAuthHeaders));
            AuthenticationHeaderValue authHeader = new(Data.Headers.s_authHeader);

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, content, authHeader).Result;

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
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, Data.s_bodyExampleData).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void PostRequestDynamicWithHeaders()
        {
            bool result = false;
            Dictionary<string, string> headers = Global.SetHeaders(nameof(PostRequestDynamicWithHeaders), Data.Headers.Post.s_key, Data.Headers.Post.s_value);

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, Data.s_bodyExampleData, headers).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }

        [Test]
        public void PostRequestDynamicWithAuthHeaders()
        {
            bool result = false;
            AuthenticationHeaderValue authHeader = new(Data.Headers.s_authHeader);

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) request = Request.Post(Data.s_jsonData.PostUrl, Data.s_bodyExampleData, authHeader).Result;

            if (request.result)
            {
                result = true;
            }

            Assert.IsTrue(result);
        }
    }
}
