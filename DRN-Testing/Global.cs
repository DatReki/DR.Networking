using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Testing
{
    internal class Global
    {
        internal static Dictionary<string, string> SetHeaders(string key, string value)
        {
            Dictionary<string, string> result = new();

            for (int i = 0; i < 6; i++)
            {
                result.Add($"{i}{key}", $"{i}{value}");
            }
            
            return result;
        }
    }
}
