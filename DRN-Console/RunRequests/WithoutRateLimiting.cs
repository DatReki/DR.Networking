using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;

using DRN_Console.Models;
using static DRN_Console.Global;
using System;

namespace DRN_Console.RunRequests
{
    internal class WithoutRateLimiting
    {
        #region Get
        internal static void Get()
        {
            List<AnswerData> answers = new List<AnswerData>()
            {
                new() { MethodType = typeof(WithoutRateLimiting), MethodName = "GetExample" },
                new() { MethodType = typeof(WithoutRateLimiting), MethodName = "GetManual", }
            };
            Answer(answers);
        }

        private static void GetExample()
        {
            string exampleWebsite = "https://www.example.com";
            ColoredConsole($"Making a request to {exampleWebsite}");
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(exampleWebsite).Result;
            MakeRequest(result);
        }

        private static void GetManual()
        {
            ColoredConsole("Write an url that you want to make a get request to");
            string url = UserInput();
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Get(url).Result;
            MakeRequest(result);
        }
        #endregion

        #region PostEncoded
        internal static void PostEncoded()
        {
            List<AnswerData> answers = new List<AnswerData>()
            {
                new() { MethodType = typeof(WithoutRateLimiting), MethodName = "PostEncodedExample" },
                new() { MethodType = typeof(WithoutRateLimiting), MethodName = "PostEncodedManual", }
            };
            Answer(answers);
        }

        private static void PostEncodedExample()
        {
            ConfigFile();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("permission", "user"),
                new KeyValuePair<string, string>("permission_description", "general-user-account")
            });

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(Json.PostUrl, content).Result;
            MakeRequest(result);
        }

        private static void PostEncodedManual()
        {
            ColoredConsole("Write an url that you want to make a post request to");
            string url = Console.ReadLine();
            ColoredConsole("Write your post parameters\n");

            List<KeyValuePair<string, string>> postValues = new();

            bool addItem = true;
            int counter = 0;
            string parameterName = null;
            while (addItem)
            {
                string answer;
                if ((counter % 2) == 0)
                {
                    ColoredConsole("Write the name of the paramter (type 'done' when you're finished)");
                    answer = Console.ReadLine();
                    if (answer == "done")
                    {
                        addItem = false;
                    }
                    else
                    {
                        parameterName = answer;
                    }
                }
                else
                {
                    ColoredConsole("Write the value of the paramter");
                    answer = UserInput();

                    postValues.Add(new KeyValuePair<string, string>(parameterName, answer));
                }
                counter++;
            }

            FormUrlEncodedContent postHeaders = new FormUrlEncodedContent(postValues);
            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(url, postHeaders).Result;
            MakeRequest(result);
        }
        #endregion

        #region PostDynamic
        internal static void PostDynamic()
        {
            ColoredConsole("Running an dynamic example request");
            PostDynamicExample();
        }

        private static void PostDynamicExample()
        {
            ConfigFile();
            Permissions p = new() { Permission = "user", Permission_Description = "general-user-account" };

            (bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result = DR.Networking.Request.Post(Json.PostUrl, p).Result;
            MakeRequest(result);
        }
        #endregion
    }
}
