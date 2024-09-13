﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.Primitives;

using MiKoSolutions.Analyzers;

namespace BenchmarkConsole
{
    // see https://adamsitnik.com/the-new-Memory-Diagnoser/
    [MemoryDiagnoser] // we need to enable it in explicit way
    [SimpleJob] // [RyuJitX64Job]
    public class Benchmarks
    {
        private const string Text = "This is a very long text to test SubString() on types such as string or StringSegment.";


        [Params(1, 5, 10, 15, 20, 25, 50)] // , */ 10, 100, 1000)] //, 10000, 100000)]
        public int Times;

        // [Benchmark(Baseline = true)]
        public string StringSubString()
        {
            return Text.Substring(5);
        }

        // [Benchmark]
        public string StringSegmentSubString()
        {
            return new StringSegment(Text).Substring(5);
        }

        [Benchmark(Baseline = true)]
        public string HumanizedConcatenated_Before()
        {
            return data.HumanizedConcatenated();
        }

        [Benchmark]
        public string HumanizedConcatenated_After()
        {
            return HumanizedConcatenatedNew(data);
        }

        private IEnumerable<string> data;

        [GlobalSetup]
        public void Setup()
        {
            data = Enumerable.Repeat("a", Times).ToArray();
        }

        public static string HumanizedConcatenatedNew(IEnumerable<string> values, string lastSeparator = "or")
        {
            var items = values.Select(_ => _.SurroundedWithApostrophe()).ToArray();

            var count = items.Length;

            switch (count)
            {
                case 0: return string.Empty;
                case 1: return items[0];
            }

            var items0 = items[0];
            var items1 = items[1];

            return HumanizedConcatenatedCore().ToString();

            StringBuilder HumanizedConcatenatedCore()
            {
                const string Separator = ", ";

                switch (count)
                {
                    case 2:
                        {
                            var builder = new StringBuilder(items0.Length + items1.Length + lastSeparator.Length + 2);

                            return builder.Append(items0).Append(' ').Append(lastSeparator).Append(' ').Append(items1);
                        }

                    case 3:
                        {
                            var items2 = items[2];

                            var builder = new StringBuilder(items0.Length + items1.Length + items2.Length + Separator.Length + lastSeparator.Length + 2);

                            return builder.Append(items0).Append(Separator).Append(items1).Append(' ').Append(lastSeparator).Append(' ').Append(items2);
                        }

                    case 4:
                        {
                            var items2 = items[2];
                            var items3 = items[3];

                            var builder = new StringBuilder(items0.Length + items1.Length + items2.Length + items3.Length + Separator.Length + Separator.Length + lastSeparator.Length + 2);

                            return builder.Append(items0).Append(Separator).Append(items1).Append(Separator).Append(items[2]).Append(' ').Append(lastSeparator).Append(' ').Append(items3);
                        }

                    default:
                        {
                            var builder = new StringBuilder(128);

                            var last = count - 1;
                            var beforeLast = last - 1;

                            for (var index = 0; index < beforeLast; index++)
                            {
                                builder.Append(items[index]).Append(Separator);
                            }

                            return builder.Append(items[beforeLast]).Append(' ').Append(lastSeparator).Append(' ').Append(items[last]);
                        }
                }
            }
        }

    }
}