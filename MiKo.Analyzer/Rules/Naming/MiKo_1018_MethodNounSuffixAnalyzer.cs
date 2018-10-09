using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1018_MethodNounSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1018";

        private static readonly KeyValuePair<string, string>[] Endings =
            {
                new KeyValuePair<string, string>("llation", "ll"),
                new KeyValuePair<string, string>("ation", "ate"),
                new KeyValuePair<string, string>("ption", "pt"),
                new KeyValuePair<string, string>("rison", "re"),
            };

        public MiKo_1018_MethodNounSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method) => TryFindBetterName(method.Name, out var betterName)
                                                                                            ? new[] { ReportIssue(method, betterName) }
                                                                                            : Enumerable.Empty<Diagnostic>();

        private static bool TryFindBetterName(string name, out string betterName)
        {
            foreach (var pair in Endings)
            {
                if (name.EndsWith(pair.Key, StringComparison.Ordinal))
                {
                    betterName = name.Substring(0, name.Length - pair.Key.Length) + pair.Value;
                    return true;
                }
            }

            betterName = name;
            return false;
        }
    }
}