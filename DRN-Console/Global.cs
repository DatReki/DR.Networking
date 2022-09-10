using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DRN_Core.Models;

namespace DRN_Console
{
    internal class Global
    {
		public static Data Json { get; set; } = ConfigFile();

		public static void MakeRequest((bool result, string errorCode, HttpContent content, HttpResponseHeaders headers) result)
		{
			if (result.result)
			{
				ColoredConsole($"Headers:");
				foreach (var header in result.headers)
				{
					ColoredConsole($"	Key: {header.Key}");
					ColoredConsole($"	Value(s):");
					foreach (var value in header.Value)
					{
						ColoredConsole($"		{value}");
					}
				}
				ColoredConsole($"\n\n\n\nContent:\n{result.content.ReadAsStringAsync().Result}");
			}
			else
			{
				ColoredConsole(result.errorCode);
			}
		}

		internal static string UserInput() => Console.ReadLine().ToLower().Trim();

		internal static void Answer(List<AnswerData> answers)
		{
			ColoredConsole(Questions.ExampleOrManul);
			switch (UserInput())
			{
				case "example":
					CallDynamicStaticMethod(answers[0].MethodType, answers[0].MethodName);
					break;
				case "manual":
					CallDynamicStaticMethod(answers[1].MethodType, answers[1].MethodName);
					break;
				default:
					Program.Restart();
					break;
			}
		}

		internal static List<KeyValuePair<string, string>> WriteValues()
        {
			List<KeyValuePair<string, string>> values = new();

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

					values.Add(new KeyValuePair<string, string>(parameterName, answer));
				}
				counter++;
			}
			return values;
		}

		internal static void CallDynamicStaticMethod(Type type, string name) => type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Invoke(null, null);

		internal static void ColoredConsole(string input)
        {
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(input);
			Console.ResetColor();
        }

		public static Data ConfigFile()
		{
			string directory = Path.GetDirectoryName(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
			string file = Path.Combine(directory, "config.json");
			Directory.CreateDirectory(directory);

			bool configFileExits = File.Exists(file);
			if (!configFileExits)
			{
				string json = JsonConvert.SerializeObject(Data.GetJsonExample(), Formatting.Indented, new StringEnumConverter());
				File.WriteAllText(file, json);
			}

			Data value = JsonConvert.DeserializeObject<Data>(File.ReadAllText(file));

			if (string.IsNullOrEmpty(value.PostUrl))
			{
				ColoredConsole($"Config file didn't exist. Add required data to the file. Location: {Path.GetFullPath(file)}");
				Environment.Exit(0);
			}
			return value;
		}

		internal class Questions
        {
			internal static readonly string ExampleOrManul = "Do you want to run an example request or a manual one? (example/manual)";

		}

		public class AnswerData
        {
			public Type MethodType { get; set; }
			public string MethodName { get; set; }
        }
	}
}
