using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1406_ValueConvertersNamespaceAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1406";

        public MiKo_1406_ValueConvertersNamespaceAnalyzer() : base(Id, SymbolKind.NamedType)
        {
        }

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsValueConverter() || symbol.IsMultiValueConverter();

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => symbol.ContainingNamespace?.Name != "Converters"
                                                                                                                        ? new[] { Issue(symbol) }
                                                                                                                        : Enumerable.Empty<Diagnostic>();
    }
}