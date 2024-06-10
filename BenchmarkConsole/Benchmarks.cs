using System;
using System.Collections.Generic;
using System.Linq;

using BenchmarkDotNet.Attributes;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Primitives;

using MiKoSolutions.Analyzers;
using MiKoSolutions.Analyzers.Rules.Documentation;

namespace BenchmarkConsole
{
    // see https://adamsitnik.com/the-new-Memory-Diagnoser/
    [MemoryDiagnoser] // we need to enable it in explicit way
//    [RyuJitX64Job]
    public class Benchmarks
    {
        private const string Text = "This is a very long text to test SubString() on types such as string or StringSegment.";

        private static SyntaxNode TypeName = SyntaxFactory.ParseTypeName("System.String");


        [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 15, 20, 25, 30)] // , */ 10, 100, 1000)] //, 10000, 100000)]
        public int Times;

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

        // [Benchmark]
        public string HumanizedConcatenated()
        {
            return data.HumanizedConcatenated();
        }

        // [Benchmark]
        public void ContainsAny_OrdinalIgnoreCase_Times()
        {
            for (var i = 0; i < Times; i++)
            {
                Text.ContainsAny(MiKo_2060_CodeFixProvider.MappedData.Value.TypeReplacementMapKeys, StringComparison.OrdinalIgnoreCase);
            }
        }

        [Benchmark]
        public bool IsAnyKind_Array() => TypeName.IsAnyKind(StrangeMarkersArray);

        [Benchmark]
        public bool IsAnyKind_Set() => TypeName.IsAnyKind(StrangeMarkersSet);

        private IEnumerable<string> data;

        private SyntaxKind[] StrangeMarkersArray;
        private HashSet<SyntaxKind> StrangeMarkersSet;

        [GlobalSetup]
        public void Setup()
        {
            StrangeMarkersArray = Enum.GetValues(typeof(SyntaxKind)).Cast<SyntaxKind>().Skip(50).Take(Times).Concat(new[]{ TypeName.Kind() }).ToArray();
            StrangeMarkersSet = new HashSet<SyntaxKind>(StrangeMarkersArray);

            data = Enumerable.Repeat("a", Times).ToArray();
        }
    }
}