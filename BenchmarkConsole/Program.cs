using System;

using BenchmarkDotNet.Running;

namespace BenchmarkConsole
{
    internal class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<Benchmarks>();

            Console.ReadKey();
        }
    }
}
