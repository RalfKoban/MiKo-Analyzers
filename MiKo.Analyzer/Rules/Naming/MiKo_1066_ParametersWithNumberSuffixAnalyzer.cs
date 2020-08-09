using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1066_ParametersWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1066";

        public MiKo_1066_ParametersWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        public static string FindBetterName(IParameterSymbol symbol) => symbol.Name.WithoutNumberSuffix();

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol) => HasIssue(symbol)
                                                                                               ? new[] { Issue(symbol) }
                                                                                               : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(IParameterSymbol symbol)
        {
            if (symbol.Name.EndsWithNumber())
            {
                if (symbol.ContainingSymbol is IMethodSymbol m && MiKo_1001_EventArgsParameterAnalyzer.IsAccepted(symbol, m))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}