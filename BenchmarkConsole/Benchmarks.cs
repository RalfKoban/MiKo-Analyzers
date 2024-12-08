using System;

using BenchmarkDotNet.Attributes;

namespace BenchmarkConsole
{
    // see https://adamsitnik.com/the-new-Memory-Diagnoser/
    [MemoryDiagnoser] // we need to enable it in explicit way
    [SimpleJob] // [RyuJitX64Job]
    public class Benchmarks
    {
        // [Params(1, 5, 10, 15, 20, 25, 50, 75, 100, 125, 150, 250, 1000, 10000, 100000)]
        [Params(100, 125, 150, 175)]
        public int Times;

       // [Benchmark(Baseline = true)]
       // public void MiKo_2023_Original() => GC.KeepAlive(new MiKoSolutions.Analyzers.Rules.Documentation.MiKo_2023_CodeFixProvider.MapData());

        // [Benchmark(Baseline = true)]
        // [Benchmark]
        // public void MiKo_2035_Original() => GC.KeepAlive(new MiKoSolutions.Analyzers.Rules.Documentation.MiKo_2035_CodeFixProvider.MapData());

        // [Benchmark(Baseline = true)]
        // [Benchmark]
        // public void MiKo_2060_Original() => GC.KeepAlive(new MiKoSolutions.Analyzers.Rules.Documentation.MiKo_2060_CodeFixProvider.MapData());

        // [Benchmark(Baseline = true)]
        // [Benchmark]
        // public void MiKo_2080_Original() => GC.KeepAlive(new MiKoSolutions.Analyzers.Rules.Documentation.MiKo_2080_CodeFixProvider.MapData());
    }
}