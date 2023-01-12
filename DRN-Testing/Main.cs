using DR.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using System.IO;
using Core = DRN_Core.Models;

namespace DRN_Testing
{
    [SetUpFixture]
    public class Main
    {
        public static Core.Data Json { get; set; }

        [OneTimeSetUp]
        public void Setup()
        {
            // Config file located in debug/bin directory of console project
            string configFile = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "DRN-Console", "bin", "Debug", "config.json");

            if (File.Exists(configFile))
            {
                Data.s_jsonData = JsonConvert.DeserializeObject<Core.Data>(File.ReadAllText(configFile));
            }
            else
            {
                string json = JsonConvert.SerializeObject(Core.Data.GetJsonExample(), Formatting.Indented, new StringEnumConverter());
                File.WriteAllText(configFile, json);

                Assert.Fail($"Config file didn't exist. Add required data to the file. Location: {Path.GetFullPath(configFile)}");
            }

            _ = new Configuration(Data.s_globalRateLimit, Data.s_rateLimits);
        }
    }
}