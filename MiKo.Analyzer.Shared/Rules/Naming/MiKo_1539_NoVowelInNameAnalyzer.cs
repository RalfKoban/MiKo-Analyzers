using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1539_NoVowelInNameAnalyzer : OverallNameAnalyzer
    {
        public const string Id = "MiKo_1539";

        public MiKo_1539_NoVowelInNameAnalyzer() : base(Id)
        {
        }

        protected override Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "") => HasIssue(symbolName)
                                                                                                              ? new[] { Issue(symbol) }
                                                                                                              : Array.Empty<Diagnostic>();

        private static bool HasIssue(string name)
        {
            if (name.Length <= 1)
            {
                return false;
            }

            switch (name)
            {
                case Constants.LambdaIdentifiers.FallbackUnderscores2:
                case Constants.LambdaIdentifiers.FallbackUnderscores3:
                case Constants.LambdaIdentifiers.FallbackUnderscores4:
                    return false;

                default:
                    return name.AsSpan().IndexOfAny("AEIOUaeiou".AsSpan()) < 0;
            }
        }
    }
}