using System;

using BenchmarkDotNet.Running;

namespace MiKoSolutions.Analyzers.BenchmarkConsole
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
