using System;

using BenchmarkDotNet.Running;

namespace BenchmarkConsole
{
    internal static class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<Benchmarks>();

            Console.ReadKey();
        }
    }
}
