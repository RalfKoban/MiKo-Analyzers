using System;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.Primitives;

using MiKoSolutions.Analyzers.Rules.Documentation;

namespace BenchmarkConsole
{
    // see https://adamsitnik.com/the-new-Memory-Diagnoser/
    [MemoryDiagnoser] // we need to enable it in explicit way
    [RyuJitX64Job]
    public class Benchmarks
    {
        private const string Text = "This is a very long text to test SubString() on types such as string or StringSegment.";

        // [Benchmark]
        public string StringSubString()
        {
            return Text.Substring(5);
        }

        // [Benchmark]
        public string StringSegmentSubString()
        {
            return new StringSegment(Text).Substring(5);
        }

        // [Benchmark]
        public void AllIndicesOf_s()
        {
            Text.AllIndicesOf("s");
        }

        // [Benchmark]
        public void AllIndicesOf_s_Span_Ordinal()
        {
            Text.AsSpan().AllIndicesOf("s", StringComparison.Ordinal);
        }

        // [Benchmark]
        public void AllIndicesOf_s_Span_OrdinalIgnoreCase()
        {
            Text.AsSpan().AllIndicesOf("s");
        }

        [Params(1, 10, 100, 1000, 10000)] // , 100000)]
        public int Times;

        [Benchmark]
        public void ContainsAny_OrdinalIgnoreCase_Times()
        {
            for (var i = 0; i < Times; i++)
            {
                Text.ContainsAny(MiKo_2060_CodeFixProvider.TypeReplacementMapKeys, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Benchmark]
        public void ContainsAnyWithSpans_OrdinalIgnoreCase_Times()
        {
            Span<string> strings = (string[])MiKo_2060_CodeFixProvider.TypeReplacementMapKeys;

            for (var i = 0; i < Times; i++)
            {
                Text.ContainsAnyWithSpans(strings, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}