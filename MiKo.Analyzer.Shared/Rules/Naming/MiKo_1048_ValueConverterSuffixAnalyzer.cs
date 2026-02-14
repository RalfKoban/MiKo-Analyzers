using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1048_ValueConverterSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1048";

        private const string Suffix = "Converter";

        public MiKo_1048_ValueConverterSuffixAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol)
        {
            if (symbol is null)
            {
                // code seems to be obfuscated or contains no valid symbol, so ignore it silently
                return false;
            }

            return symbol.IsValueConverter() || symbol.IsMultiValueConverter();
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation)
        {
            var symbolName = symbol.Name;

            if (symbolName.EndsWith(Suffix, StringComparison.Ordinal))
            {
                return Array.Empty<Diagnostic>();
            }

            return new[] { Issue(symbol, Suffix, CreateBetterNameProposal(symbolName + Suffix)) };
        }
    }
}