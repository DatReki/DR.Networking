#if DEBUG
using BenchmarkDotNet.Configs;
#endif
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await Requests.Setup();
#if DEBUG
            BenchmarkSwitcher.FromAssembly(typeof(Requests).Assembly).Run(args, new DebugInProcessConfig());
#else
            BenchmarkRunner.Run<Requests>();
#endif
            Console.ReadLine();
        }
    }
}
