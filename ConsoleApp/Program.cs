using DR.Networking.Models;
using System.Reflection;

namespace ConsoleApp
{
    internal class Program
    {
        internal static bool Finished { get; private set; } = false;

        static async Task Main(string[] _)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            await SelectOption();

            int counter = 0;
            while(counter <= 100)
            {
                await Task.Delay(10);

                if (Finished)
                    break;
                else
                    counter++;
            }

            Console.WriteLine(string.Empty);
            Console.WriteLine("Done!");

            Console.ReadKey();
            Environment.Exit(0);
        }

        private static async Task SelectOption()
        {
            List<string> namespaces = [..Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Select(x => x.Namespace)
                .Distinct()
                .Select(x => string.IsNullOrEmpty(x) ? string.Empty : x)
                .Where(x => x.Length > 0)];

            List<Type> classes = [..Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.Namespace == "ConsoleApp.Examples")
                .Where(x => x.FullName != null && !x.FullName.Contains('+') && x.IsClass && !string.IsNullOrWhiteSpace(x.AssemblyQualifiedName))];

            Console.WriteLine($"What type of request do you want to make?");
            for (int i = 0; i < classes.Count; i++)
                Console.WriteLine($"[{i}] {classes[i].Name}");

            var selectedType = ReadUserInput(Console.ReadLine(), classes.Select(x => x.Name));
            if (selectedType == -1)
            {
                await Restart();
                return;
            }
            else
                Console.WriteLine(string.Empty);

            var methods = classes[selectedType]
                .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .Where(x => x.IsAssembly)
                .ToList();

            Console.WriteLine($"Which example do you want to run?");
            for (int i = 0; i < methods.Count; i++)
                Console.WriteLine($"[{i}]: {methods[i].Name}");

            var selectedMethod = ReadUserInput(Console.ReadLine(), methods.Select(x => x.Name));
            if (selectedMethod == -1)
            {
                await Restart();
                return;
            }
            else
                Console.WriteLine(string.Empty);

            var method = methods[selectedMethod];
            method.Invoke(classes[selectedType], null);
        }

        internal static int ReadUserInput(string? input, IEnumerable<string> options)
        {
            int result = -1;
            if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int inputNumber) && inputNumber < options.Count())
                result = inputNumber;

            return result;
        }

        internal static void ShowResult(ResultData data)
        {
            Console.WriteLine($"Request status: {data.Success}");
            Console.WriteLine($"Http status code: {data.StatusCode}");
            
            if (!data.Success)
            {
                Console.WriteLine($"Error type: {Enum.GetName(typeof(ErrorType), data.ErrorType)}");
                Console.WriteLine($"Error type: {data.Error}");
            }

            Finished = true;
        }

        internal static async Task Restart()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Your answer does not match any of the available options.\nType any key to try again!");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();

            await Main([]);
        }
    }
}
