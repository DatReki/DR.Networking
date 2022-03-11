using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRN_Console.RunRequests
{
    internal class WithRateLimiting
    {
        static WithRateLimiting()
        {
            Console.WriteLine("Do you want to specify you own data/ratelimits or use example data? (example/own)");
            switch (Console.ReadLine())
            {
                case "example":
                    break;
                case "own":
                    s_useExample = false;
                    break;
                default:
                    Program.Restart();
                    break;
            }
        }

        private static bool s_useExample = true;

        internal static void Get()
        {
            Console.WriteLine("Get");
        }

        internal static void PostEncoded()
        {
            Console.WriteLine("PostEncoded");
        }

        internal static void PostDynamic()
        {
            Console.WriteLine("PostDynamic");
        }

        private static void SetRateLimit()
        {
            if (s_useExample)
            {

            }
            else
            {

            }
        }
    }
}
