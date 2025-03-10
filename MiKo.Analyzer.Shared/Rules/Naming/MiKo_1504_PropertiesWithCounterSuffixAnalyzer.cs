using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MiKo_1504_PropertiesWithCounterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1504";

        public MiKo_1504_PropertiesWithCounterSuffixAnalyzer() : base(Id, SymbolKind.Property)
        {
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => base.ShallAnalyze(symbol) && symbol.Type.TypeKind == TypeKind.Struct;

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith("Counter", StringComparison.OrdinalIgnoreCase))
            {
                var betterName = "Counted" + symbolName.WithoutSuffix("Counter").ToUpperCaseAt(0);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}