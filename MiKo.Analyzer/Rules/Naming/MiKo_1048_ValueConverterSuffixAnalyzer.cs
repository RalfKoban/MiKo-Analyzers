using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsValueConverter() || symbol.IsMultiValueConverter();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol) => symbol.Name.EndsWith(Suffix, StringComparison.Ordinal)
                                                                                           ? Enumerable.Empty<Diagnostic>()
                                                                                           : new[] { Issue(symbol, Suffix) };
    }
}