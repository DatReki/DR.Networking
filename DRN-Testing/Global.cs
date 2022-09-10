using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Testing
{
    internal class Global
    {
        internal static Dictionary<string, string> SetHeaders(string methodName, string key, string value)
        {
            Dictionary<string, string> result = new();

            for (int i = 0; i < 6; i++)
            {
                result.Add($"{methodName}_{i}{key}", $"{methodName}_{i}{value}");
            }

            return result;
        }

        internal static FormUrlEncodedContent SetContent(string methodName)
        {
            return new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>($"{methodName}_test_1", "SetContent"),
                new KeyValuePair<string, string>($"{methodName}_test_2", "SetContent")
            });
        }
    }
}
