using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_1507_ParametersWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1507";

        public MiKo_1507_ParametersWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Parameter)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            if (HasIssue(symbol))
            {
                var betterName = "counted" + symbol.Name.WithoutSuffix("Counter").ToUpperCaseAt(0);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        private static bool HasIssue(IParameterSymbol symbol) => symbol.Type.TypeKind == TypeKind.Struct && symbol.Name.EndsWith("Counter", StringComparison.OrdinalIgnoreCase);
    }
}