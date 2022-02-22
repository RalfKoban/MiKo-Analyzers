using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1085_ParametersWithNumberSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1085";

        public MiKo_1085_ParametersWithNumberSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol) => symbol.Name.WithoutNumberSuffix();

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => HasIssue(symbol)
                                                                                                                        ? new[] { Issue(symbol) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();

        private static bool HasIssue(IParameterSymbol symbol)
        {
            if (symbol.Name.EndsWithCommonNumber())
            {
                if (MiKo_1001_EventArgsParameterAnalyzer.IsAccepted(symbol))
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}