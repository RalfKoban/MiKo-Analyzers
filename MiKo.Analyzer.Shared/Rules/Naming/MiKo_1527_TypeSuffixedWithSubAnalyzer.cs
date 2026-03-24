using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1527_TypeSuffixedWithSubAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1527";

        public MiKo_1527_TypeSuffixedWithSubAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            const string Suffix = "Sub";

            var symbolName = symbol.Name;

            if (symbolName.Length > Suffix.Length && symbolName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                var betterName = symbolName.Substring(0, symbolName.Length - Suffix.Length);

                return new[] { Issue(symbol, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }
    }
}