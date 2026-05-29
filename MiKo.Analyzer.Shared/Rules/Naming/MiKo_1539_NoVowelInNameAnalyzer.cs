using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1539_NoVowelInNameAnalyzer : OverallNameAnalyzer
    {
        public const string Id = "MiKo_1539";

        private static readonly HashSet<string> AllowedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                                                                   {
                                                                       Constants.LambdaIdentifiers.FallbackUnderscores2,
                                                                       Constants.LambdaIdentifiers.FallbackUnderscores3,
                                                                       Constants.LambdaIdentifiers.FallbackUnderscores4,
                                                                       "By",
                                                                       "Try",
                                                                       "XML",
                                                                       "HTML",
                                                                       "HTTP",
                                                                       "HTTPS",
                                                                       "tcs", // TaskCancellationSource
                                                                       "CRC",
                                                                       "TCP",
                                                                   };

        public MiKo_1539_NoVowelInNameAnalyzer() : base(Id)
        {
        }

        // overridden as it is not important how the prefix looks like, so we can save some memory by not creating a new string for the field name
        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol.Name, symbol);

        protected override Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "") => HasIssue(symbolName)
                                                                                                              ? new[] { Issue(symbol) }
                                                                                                              : Array.Empty<Diagnostic>();

        private static bool HasIssue(string name)
        {
            if (name.Length > 1 && name.AsSpan().IndexOfAny("AEIOUaeiou".AsSpan()) < 0)
            {
                return AllowedNames.Contains(name) is false;
            }

            return false;
        }
    }
}